using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.identity.usuarios;

/// <summary>
/// Entidad de control de los roles por cuenta fiscal de un usuario.
/// </summary>
[ExcludeFromCodeCoverage]
public class RolCuentaFiscal
{
    /// <summary>
    /// Identificador unico de la entrada.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Identificador del usuario.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Identificador de la cuenta fiscal.
    /// </summary>
    public Guid CuentaFiscalId { get; set; }

    /// <summary>
    /// Lista de roles en forma de texto separados por comas.
    /// </summary>
    required public string Roles { get; set; }
}
