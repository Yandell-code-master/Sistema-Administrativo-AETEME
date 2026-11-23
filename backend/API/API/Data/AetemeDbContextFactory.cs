using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace API.Data;

/// <summary>
/// Solo se usa en tiempo de diseño para `dotnet ef migrations`. La cadena se toma del
/// entorno (ConnectionStrings__DefaultConnection) o del valor de desarrollo local.
/// </summary>
public sealed class AetemeDbContextFactory : IDesignTimeDbContextFactory<AetemeDbContext>
{
    public AetemeDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=aeteme_dev;Username=aeteme_admin;Password=aeteme_dev_password";

        var options = new DbContextOptionsBuilder<AetemeDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AetemeDbContext(options);
    }
}
