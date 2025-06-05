using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.identity.usuarios;

/// <summary>
/// DTO Para realizar el cambio de contraseña.
/// </summary>
[ExcludeFromCodeCoverage]
public class DTOCambiarContrasena
{
    /// <summary>
    /// Usuario para realizar el cambio.
    /// </summary>
    public Guid? UsuarioId { get; set; }

    /// <summary>
    /// Nueva contraseña.
    /// </summary>
    required public string Nueva { get; set; }

    /// <summary>
    /// Contraseña actual.
    /// </summary>
    public string? Actual { get; set; }
}
