namespace API.Models;

public sealed class Usuario
{
    public int IdUsuario { get; set; }

    /// <summary>Se persiste como texto (varchar) mediante una conversión de EF Core.</summary>
    public RolUsuario Rol { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public byte[] ContrasenaHash { get; set; } = [];

    public byte[] ContrasenaSalt { get; set; } = [];

    public bool CorreoVerificado { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public ICollection<TokenVerificacionCorreo> TokensVerificacionCorreo { get; set; } = [];

    public ICollection<RegistroAuditoria> RegistrosAuditoria { get; set; } = [];
}
