namespace API.Models;

public sealed class RegistroAuditoria
{
    public long IdRegistroAuditoria { get; set; }

    public int IdUsuario { get; set; }

    public Usuario Usuario { get; set; } = null!;

    /// <summary>Nombre del módulo donde ocurrió la acción (texto, sin catálogo).</summary>
    public string Modulo { get; set; } = null!;

    public string Accion { get; set; } = null!;

    public DateTime Fecha { get; set; }
}
