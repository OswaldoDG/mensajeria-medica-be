using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.identity.registro;

namespace mensajeriamedica.model.identity.usuarios;

/// <summary>
/// DTO para la gestion de cuentas de usuario.
/// </summary>
[ExcludeFromCodeCoverage]
public class CuentaUsuario
{
    /// <summary>
     /// Identificador unido de la cuenta.
     /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Correo asociado a la cuenta.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Nombre completo de la persona.
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Estado de la cuenta.
    /// </summary>
    public EstadoCuenta Estado { get; set; } = EstadoCuenta.PendienteConfirmacion;

    /// <summary>
    /// Fecha en que el usuario realizó la solicitud de inscripción.
    /// </summary>
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de confirmación de la solicitud de inscripcion.
    /// </summary>
    public DateTime? FechaActivacion { get; set; }

    /// <summary>
    /// Tipo de cuenta.
    /// </summary>
    public TipoCuenta TipoCuenta { get; set; } = TipoCuenta.None;

    /// <summary>
    /// Id de la cuenta fiscal asociada al usuario.
    /// </summary>
    public Guid? CuentaFiscalId { get; set; }

    /// <summary>
    /// Si el usuario es loginless especifica la fecha de caducidad del token.
    /// </summary>
    public DateTime? FechaCaducidadTokenLoginless { get; set; }

    /// <summary>
    /// Identificador unico del usuario creador de la cuenta.
    /// </summary>
    public Guid? CreadorId { get; set; }

    /// <summary>
    /// Roles del usaurio.
    /// </summary>
    public List<string> Roles { get; set; } = [];
}
