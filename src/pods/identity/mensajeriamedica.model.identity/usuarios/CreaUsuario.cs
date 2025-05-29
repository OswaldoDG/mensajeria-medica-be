using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.identity.registro;

namespace contabee.model.identity.usuarios;

/// <summary>
/// DTO para la cracion de usaurios.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreaUsuario
{
    /// <summary>
    /// Email del usuario.
    /// </summary>
    public string? Email { get; set; }

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
    /// Tipo de cuenta del usuario.
    /// </summary>
    public TipoCuenta TipoCuenta { get; set; }

    /// <summary>
    /// Identificador de la cuenta fiscal.
    /// </summary>
    public Guid? CuentaFiscalId { get; set; }
}
