using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Correo { get; set; } = null!;

    [Required]
    [MaxLength(128)]
    public string Contrasena { get; set; } = null!;
}

public sealed class AuthResponse
{
    public required string AccessToken { get; init; }
    public required int ExpiresIn { get; init; }
    public required UsuarioResponse Usuario { get; init; }
}

public sealed class UsuarioResponse
{
    public required int IdUsuario { get; init; }
    public required string Rol { get; init; }
    public required string NombreCompleto { get; init; }
    public required string Correo { get; init; }
}
