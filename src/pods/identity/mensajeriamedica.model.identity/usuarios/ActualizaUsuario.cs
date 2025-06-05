using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.identity.registro;

namespace mensajeriamedica.model.identity.usuarios;

/// <summary>
/// DTO para la actualizacion de usaurios.
/// </summary>
[ExcludeFromCodeCoverage]
public class ActualizaUsuario
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Email del usuario.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Numer telefonico.
    /// </summary>
    public string? Telefono { get; set; }
}
