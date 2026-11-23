using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public sealed class AuthController(AuthService authService, IHostEnvironment environment) : ControllerBase
{
    private const string RefreshCookieName = "aeteme_refresh";
    private static readonly TimeSpan CookieLifetime = TokenService.RefreshTokenSlidingLifetime;

    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion(
        [FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var resultado = await authService.IniciarSesionAsync(request, cancellationToken);
        if (resultado is null)
        {
            return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });
        }

        AppendRefreshCookie(resultado.Value.RefreshToken);
        return Ok(resultado.Value.Response);
    }

    [HttpPost("renovar")]
    public async Task<IActionResult> RenovarSesion(CancellationToken cancellationToken)
    {
        var token = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new { mensaje = "No hay sesión activa." });
        }

        var resultado = await authService.RenovarAsync(token, cancellationToken);
        if (resultado is null)
        {
            DeleteRefreshCookie();
            return Unauthorized(new { mensaje = "La sesión expiró. Inicie sesión nuevamente." });
        }

        AppendRefreshCookie(resultado.Value.RefreshToken);
        return Ok(resultado.Value.Response);
    }

    [Authorize]
    [HttpPost("cerrar")]
    public async Task<IActionResult> CerrarSesion(CancellationToken cancellationToken)
    {
        var token = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(token))
        {
            await authService.CerrarSesionAsync(token, cancellationToken);
        }

        DeleteRefreshCookie();
        return NoContent();
    }

    private void AppendRefreshCookie(string token)
    {
        Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = environment.IsProduction(),
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            Path = "/api/auth",
            MaxAge = CookieLifetime
        });
    }

    private void DeleteRefreshCookie()
    {
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = environment.IsProduction(),
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            Path = "/api/auth"
        });
    }
}
