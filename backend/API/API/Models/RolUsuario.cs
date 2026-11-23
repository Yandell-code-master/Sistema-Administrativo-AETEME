namespace API.Models;

/// <summary>
/// Roles con cuenta real en el sistema. Visitante no se registra: es acceso anónimo al portal público.
/// </summary>
public enum RolUsuario
{
    Administrador,
    SuperAdministrador
}
