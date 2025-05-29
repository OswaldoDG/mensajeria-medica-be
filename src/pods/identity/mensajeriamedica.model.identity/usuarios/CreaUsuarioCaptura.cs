using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.identity.registro;

namespace contabee.model.identity.usuarios;

/// <summary>
/// DTO para la cracion de usaurios.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreaUsuarioCaptura
{
    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Contrasena del usuario.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Numer telefonico.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Identificador de la cuenta fiscal.
    /// </summary>
    public Guid CuentaFiscalId { get; set; }
}
