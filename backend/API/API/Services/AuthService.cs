using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

/// <summary>
/// Login, renovación con rotación y cierre de sesión. Las cookie de refresh se gestionan
/// en el controller; aquí solo se persisten hashes y se aplican las reglas de negocio.
/// </summary>
public sealed class AuthService(AetemeDbContext db, TokenService tokenService)
{
    public async Task<(AuthResponse Response, string RefreshToken)?> IniciarSesionAsync(
        LoginRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await db.Usuarios
            .SingleOrDefaultAsync(
                u => u.Correo == request.Correo && u.Activo,
                cancellationToken);

        if (usuario is null ||
            !PasswordHasher.Verify(request.Contrasena, usuario.ContrasenaHash, usuario.ContrasenaSalt))
        {
            return null;
        }

        var (refreshToken, hash, expiracion) = tokenService.GenerateRefreshToken();
        db.RefreshTokens.Add(new RefreshToken
        {
            IdUsuario = usuario.IdUsuario,
            TokenHash = hash,
            FechaExpiracion = expiracion
        });
        await db.SaveChangesAsync(cancellationToken);

        return (BuildResponse(usuario), refreshToken);
    }

    /// <summary>
    /// Renueva la sesión. Devuelve null cuando el token no existe, está vencido o fue revocado.
    /// En ese último caso revoca todos los tokens activos del usuario (familia).
    /// </summary>
    public async Task<(AuthResponse Response, string RefreshToken)?> RenovarAsync(
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var hash = TokenService.Hash(refreshToken);
        var registro = await db.RefreshTokens
            .Include(t => t.Usuario)
            .SingleOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (registro is null)
        {
            return null;
        }

        if (registro.Revocado || registro.FechaExpiracion <= DateTime.UtcNow)
        {
            await RevocarFamiliaAsync(registro.IdUsuario, cancellationToken);
            return null;
        }

        if (!registro.Usuario.Activo)
        {
            return null;
        }

        // Rotación: el token usado se revoca y se emite uno nuevo con ventana sliding.
        registro.Revocado = true;
        var (nuevoToken, nuevoHash, expiracion) = tokenService.GenerateRefreshToken();
        db.RefreshTokens.Add(new RefreshToken
        {
            IdUsuario = registro.IdUsuario,
            TokenHash = nuevoHash,
            FechaExpiracion = expiracion
        });

        await db.SaveChangesAsync(cancellationToken);

        return (BuildResponse(registro.Usuario), nuevoToken);
    }

    public async Task CerrarSesionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var hash = TokenService.Hash(refreshToken);
        var registro = await db.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (registro is null || registro.Revocado)
        {
            return;
        }

        registro.Revocado = true;
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task RevocarFamiliaAsync(int idUsuario, CancellationToken cancellationToken)
    {
        var tokens = await db.RefreshTokens
            .Where(t => t.IdUsuario == idUsuario && !t.Revocado)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revocado = true;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private AuthResponse BuildResponse(Usuario usuario) => new()
    {
        AccessToken = tokenService.GenerateAccessToken(usuario),
        ExpiresIn = (int)TokenService.AccessTokenLifetime.TotalSeconds,
        Usuario = new UsuarioResponse
        {
            IdUsuario = usuario.IdUsuario,
            Rol = usuario.Rol.ToString(),
            NombreCompleto = usuario.NombreCompleto,
            Correo = usuario.Correo
        }
    };
}
