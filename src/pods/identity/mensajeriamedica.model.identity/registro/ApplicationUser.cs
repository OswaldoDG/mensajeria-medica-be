using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.model.identity.usuarios;
using Microsoft.AspNetCore.Identity;

namespace contabee.model.identity.registro;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
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

    /// <summary>
    /// Tipo de cuenta.
    /// </summary>
    [Required]
    public TipoCuenta TipoCuenta { get; set; } = TipoCuenta.None;

    /// <summary>
    /// Nombre completo de la persona.
    /// </summary>
    [MaxLength(200)]
    public string? Nombre { get; set; }

    /// <summary>
    /// Identificador único de la cuenta fiscal a la que pertenece.
    /// </summary>
    public Guid? CuentaFiscalId { get; set; }

    /// <summary>
    /// Identificador unico del usuario creador de la cuenta.
    /// </summary>
    public Guid? CreadorId { get; set; }

    [JsonIgnore]
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
    public List<DispositivoUsuario> DispositivosUsuario { get; set; }
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
}
