namespace API.Models;

public sealed class Patrocinador
{
    public int IdPatrocinador { get; set; }

    public string Nombre { get; set; } = null!;

    public string RutaLogotipo { get; set; } = null!;

    public string EnlaceWeb { get; set; } = null!;

    public string UrlInformacion { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public string Contacto { get; set; } = null!;

    public string? Observaciones { get; set; }

    public string BeneficiosAcordados { get; set; } = null!;

    public bool Activo { get; set; } = true;

    public DateTime? FechaUltimoRecordatorio { get; set; }

    public string Categoria { get; set; } = null!;

    public string TipoPatrocinio { get; set; } = null!;
}
