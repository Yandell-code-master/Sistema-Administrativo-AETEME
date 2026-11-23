namespace API.Models;

public sealed class InformacionInstitucional
{
    /// <summary>Registro único: siempre 1.</summary>
    public int IdInformacionInstitucional { get; set; }

    public string Nombre { get; set; } = null!;

    /// <summary>Texto institucional (misión, visión, valores y objetivos).</summary>
    public string Texto { get; set; } = null!;
}
