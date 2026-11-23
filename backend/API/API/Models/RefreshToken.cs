namespace API.Models;

public sealed class RefreshToken
{
    public int IdRefreshToken { get; set; }

    public int IdUsuario { get; set; }

    public Usuario Usuario { get; set; } = null!;

    /// <summary>Resumen SHA-512 del token; el valor original solo viaja en la cookie HttpOnly.</summary>
    public byte[] TokenHash { get; set; } = [];

    public DateTime FechaExpiracion { get; set; }

    public bool Revocado { get; set; }
}
