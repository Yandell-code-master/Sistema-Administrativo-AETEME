namespace API.Models;

public sealed class TokenVerificacionCorreo
{
    public int IdTokenVerificacion { get; set; }

    public int IdUsuario { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public byte[] TokenHash { get; set; } = [];

    public DateTime FechaExpiracion { get; set; }

    public DateTime? FechaUso { get; set; }
}
