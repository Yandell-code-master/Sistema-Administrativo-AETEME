using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

/// <summary>
/// Emite y valida los tokens de la autenticación de doble token.
/// - JWT de acceso: vigencia fija de 15 minutos.
/// - Refresh token: opaco, se guarda solo su hash SHA-512; sliding de 15 minutos
///   de inactividad y rotación de un solo uso.
/// </summary>
public sealed class TokenService(IConfiguration configuration)
{
    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan RefreshTokenSlidingLifetime = TimeSpan.FromMinutes(15);

    public string GenerateAccessToken(Usuario usuario)
    {
        var key = GetSigningKey();
        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim("rol", usuario.Rol.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = configuration["Jwt:Issuer"] ?? "AETEME",
            Audience = configuration["Jwt:Audience"] ?? "AETEME-Web",
            IssuedAt = now,
            NotBefore = now,
            Expires = now.Add(AccessTokenLifetime),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityTokenHandler().CreateToken(descriptor));
    }

    /// <summary>Devuelve el token en claro (viaja en la cookie) y su hash (se persiste).</summary>
    public (string Token, byte[] Hash, DateTime FechaExpiracion) GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (token, Hash(token), DateTime.UtcNow.Add(RefreshTokenSlidingLifetime));
    }

    public static byte[] Hash(string token) => SHA512.HashData(Encoding.UTF8.GetBytes(token));

    public bool TryValidateAccessToken(string token, out int idUsuario)
    {
        idUsuario = 0;
        var key = GetSigningKey();

        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"] ?? "AETEME",
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"] ?? "AETEME-Web",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return int.TryParse(sub, out idUsuario);
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private SymmetricSecurityKey GetSigningKey()
    {
        var secret = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Key debe configurarse con al menos 32 caracteres.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
}
