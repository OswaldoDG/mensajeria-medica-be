using System.Diagnostics.CodeAnalysis;

namespace contabee.model.identity.tokenloginless;

/// <summary>
/// Dto para la creacion de un token loginless.
/// </summary>
[ExcludeFromCodeCoverage]
public class SolictudTokenLoginless
{
    /// <summary>
    /// Token de vinculación.
    /// </summary>
    public string TokenVinculacion { get; set; }

    /// <summary>
    /// Nombre de quien realiza la solicitud.
    /// </summary>
    public required string Nombre { get; set; }

    /// <summary>
    /// Email de quien realiza la solicitud de la solicitud.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Telefono de quien realiza la solicitud.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Fecha de caducidad del token.
    /// </summary>
    public DateTime? Caducidad { get; set; }

    /// <summary>
    /// Identificador de la cuenta fiscal que emitió el acecs.
    /// </summary>
    public Guid? CuentaFiscalId { get; set; }
}
