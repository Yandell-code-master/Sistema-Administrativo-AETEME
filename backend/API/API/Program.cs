using System.Text;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=aeteme_dev;Username=aeteme_admin;Password=aeteme_dev_password";

builder.Services.AddDbContext<AetemeDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key debe configurarse con al menos 32 caracteres.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "AETEME",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AETEME-Web",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiter.QueueLimit = 0;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await AplicarMigracionesYInicializarAsync(app);

app.Run();

static async Task AplicarMigracionesYInicializarAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<AetemeDbContext>();

    try
    {
        // Las migraciones se aplican manualmente con `dotnet dotnet-ef database update`.
        var seedEmail = app.Configuration["SeedSuperAdmin:Correo"] ?? "superadmin@aeteme.local";
        var seedPassword = app.Configuration["SeedSuperAdmin:Password"];
        if (app.Environment.IsDevelopment() && !string.IsNullOrWhiteSpace(seedPassword))
        {
            if (!await db.Usuarios.AnyAsync(u => u.Correo == seedEmail))
            {
                var (hash, salt) = PasswordHasher.Hash(seedPassword);
                db.Usuarios.Add(new Usuario
                {
                    Rol = RolUsuario.SuperAdministrador,
                    NombreCompleto = "Super Administrador AETEME",
                    Correo = seedEmail,
                    ContrasenaHash = hash,
                    ContrasenaSalt = salt,
                    CorreoVerificado = true,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                });

                await db.SaveChangesAsync();
                logger.LogInformation("SuperAdministrador inicial creado para {Correo}", seedEmail);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "No se pudo inicializar la base de datos.");
        throw;
    }
}
