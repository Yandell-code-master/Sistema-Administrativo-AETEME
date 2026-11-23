namespace API.Models;

public sealed class Evento
{
    public int IdEvento { get; set; }

    public string Titulo { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public string Lugar { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public bool EsPublico { get; set; }

    public string? EnlaceInscripcion { get; set; }

    public bool Activo { get; set; } = true;

    public string Categoria { get; set; } = null!;
}
