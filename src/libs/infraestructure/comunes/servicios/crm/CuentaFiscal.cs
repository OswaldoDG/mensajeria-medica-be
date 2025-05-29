using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm;

/// <summary>
/// Almacena los detalles de una cuenta fiscal mexicana asociada a uan RFC.
/// </summary>
[ExcludeFromCodeCoverage]
public class CuentaFiscal
{
    /// <summary>
    /// Identificador único de la cuenta fiscal creada por un usuario.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Usuario queda de alta el RFC y propiedad de navegacion para la cuenta, es clave secundaria para el repositorio .
    /// </summary>
    public Guid CuentaUsuarioId { get; set; }

    /// <summary>
    /// Registro federal de contribuyentes, es clave secundaria para el repositorio junto con UsaurioId.
    /// </summary>
    required public string RFC { get; set; }

    /// <summary>
    /// Define si la cuenta es para uso compartido o no.
    /// </summary>
    public bool Compartida { get; set; } = false;

    /// <summary>
    /// Nombre o razón social.
    /// </summary>
    [MaxLength(255)]
    public string? Nombre { get; set; }

    /// <summary>
    /// Primer apellido.
    /// </summary>
    public string? Apellido1 { get; set; }

    /// <summary>
    /// Segundo apellido.
    /// </summary>
    public string? Apellido2 { get; set; }

    /// <summary>
    /// Fecha de nacimiento.
    /// </summary>
    public string? Nacimiento { get; set; }

    /// <summary>
    /// Nombre comercial.
    /// </summary>
    [MaxLength(500)]
    public string? NombreComercial { get; set; }

    /// <summary>
    /// Fecha de inicio de operaciones.
    /// </summary>
    public string? InicioOperaciones { get; set; }

    /// <summary>
    /// Estado de la cuenta por ejemplo ACTIVO.
    /// </summary>
    public string? Estado { get; set; }

    /// <summary>
    /// Fecha de último cambio de situación.
    /// </summary>
    public string? UltimoCambio { get; set; }

    /// <summary>
    /// Régimen fiscal.
    /// </summary>
    public string? Regimen { get; set; }

    /// <summary>
    /// Régimen fiscal.
    /// </summary>
    public string? FechaAlta { get; set; }

    /// <summary>
    /// Tipode persona para la cuenta fiscal.
    /// </summary>
    public TipoPersonaFiscal Tipo { get; set; }

    /// <summary>
    /// Clave para del regimen fical para la cuenta.
    /// </summary>
    public string? ClaveRegimenFiscal { get; set; }

    /// <summary>
    /// Direcciones de la cuenta fiiscal.
    /// </summary>
    public List<DireccionFiscal> Direcciones { get; set; } = [];
}

/// <summary>
/// Tipo de persona fiscal.
/// </summary>
public enum TipoPersonaFiscal
{
    /// <summary>
    /// Persona física.
    /// </summary>
    Fisica = 0,

    /// <summary>
    /// PErsona moral.
    /// </summary>
    Moral = 1
}