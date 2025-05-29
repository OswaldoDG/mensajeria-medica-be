using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.identity.registro;

namespace contabee.model.identity.administracion;

/// <summary>
/// Datos compartidos para la administarcion de cuentas.
/// </summary>
[ExcludeFromCodeCoverage]
public class CuentaBase
{
    /// <summary>
    /// Identificador unido de la cuenta.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Correo asociado a la cuenta.
    /// </summary>
    required public string Email { get; set; }

    /// <summary>
    /// Nombre completo de la persona.
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Estado de la cuenta.
    /// </summary>
    public EstadoCuenta Estado { get; set; } = EstadoCuenta.PendienteConfirmacion;

    /// <summary>
    /// Fecha en que el usuario realizó la solicitud de inscripción
    /// </summary>
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de confirmación de la solicitud de inscripcion
    /// </summary>
    public DateTime? FechaActivacion { get; set; }
}
