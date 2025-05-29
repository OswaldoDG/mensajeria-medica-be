using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm;

/// <summary>
/// Estado de la cuenta de usurio.
/// </summary>
public enum EstadoCuenta
{
    Activa,
    Inactiva
}

/// <summary>
/// Define el tipo de cuenta.
/// </summary>
public enum TipoCuenta
{
    /// <summary>
    /// La es administrada por el usuario asociado y regularmente es quien realiza los pagos y funge como contacto cliente
    /// </summary>
    Primaria,

    /// <summary>
    /// SOn cuentas que dependen de una cuenta primaria por ejemplo los agentes de ventas y empleados
    /// </summary>
    Secundaria
}

/// <summary>
/// Vinculo entr cuentas de usuario  ycuentas fiscales.
/// </summary>
[ExcludeFromCodeCoverage]
public class AsociacionCuentaFiscal
{
    /// <summary>         /// Iedntificador unico de la asociacion.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Identificador único de la cuenta fiscal.
    /// </summary>
    public Guid CuentaFiscalId { get; set; }

    /// <summary>
    /// Registro federal de contribuyentes asociado a la cuenta.
    /// </summary>
    [MaxLength(15)]
    required public string RFC { get; set; }

    /// <summary>
    /// Tipo de cuenta asociada al usuario.
    /// </summary>
    public TipoCuenta TipoCuenta { get; set; }

    /// <summary>
    /// Fecha en la que se asocia la cuenta.
    /// </summary>
    public DateTime FechaAsociacion { get; set; }

    /// <summary>
    /// Determina si la asociación está activa por ejemplo para cuentas secundarias
    /// la activación puede llevar una validación.
    /// </summary>
    public bool Activa { get; set; }

    /// <summary>
    /// Indica si el acceso a la cuenta caduca.
    /// </summary>
    public bool Caduca { get; set; } = false;

    /// <summary>
    /// Especifica la fecha opcional de caducidad.
    /// </summary>
    public DateTime? Caducidad { get; set; }

    /// <summary>
    /// Roles del usuario en la cuenta.
    /// </summary>
    public List<string> Roles { get; set; } = [];
}
