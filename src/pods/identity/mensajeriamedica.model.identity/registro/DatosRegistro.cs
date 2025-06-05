using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.identity.registro;

/// <summary>
/// Dto para obtener los datos de un registro pendiente.
/// </summary>
[ExcludeFromCodeCoverage]
public class DatosRegistro
{
    /// <summary>
    /// Email del registro.
    /// </summary>
    required public string Email { get; set; }
}
