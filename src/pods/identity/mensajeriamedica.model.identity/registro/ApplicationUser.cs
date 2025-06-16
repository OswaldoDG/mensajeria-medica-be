using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace mensajeriamedica.model.identity.registro;

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
    /// Nombre completo de la persona.
    /// </summary>
    [MaxLength(200)]
    public string? Nombre { get; set; }
}
