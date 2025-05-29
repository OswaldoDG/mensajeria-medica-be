using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.api.identity.models;

/// <summary>
/// DTO de la configuracion de identidad.
/// </summary>
[ExcludeFromCodeCoverage]
public class ConfiguracionServicioIdentidad
{
    /// <summary>
    /// Ruta al certificado para las operaciones de crifrado de JWT, SOLO SE UTILZIAN POR EL SERVER DE IDENTITY.
    /// </summary>
    public string? EncryptionCertificate { get; set; }

    /// <summary>
    /// Ruta al certificado para las operaciones de firma de JWT, SOLO SE UTILZIAN POR EL SERVER DE IDENTITY.
    /// </summary>
    public string? SigningCertificate { get; set; }

    /// <summary>
    /// Contraseña para el certificado de firma, SOLO SE UTILZIAN POR EL SERVER DE IDENTITY.
    /// </summary>
    public string? SigningPassword { get; set; }


    /// <summary>
    /// Especifica si el servidor de indentidad cifra el payload del JWT.
    /// </summary>
    public bool JWTCifrado { get; set; }

    /// <summary>
    /// Url del servidor de identidad.
    /// </summary>
    public string? UrlToken { get; set; }
}
