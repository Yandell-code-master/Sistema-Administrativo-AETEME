namespace API.Models;

public sealed class Noticia
{
    public int IdNoticia { get; set; }

    public string Titulo { get; set; } = null!;

    public string Cuerpo { get; set; } = null!;

    public DateTime? FechaPublicacion { get; set; }

    public string? RutaImagenDestacada { get; set; }

    public bool Activo { get; set; } = true;

    public string Categoria { get; set; } = null!;

    /// <summary>Se persiste como texto (varchar) mediante una conversión de EF Core.</summary>
    public EstadoNoticia Estado { get; set; }
}
