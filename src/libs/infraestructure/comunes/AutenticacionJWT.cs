using System.Diagnostics.CodeAnalysis;

namespace comunes;

/// <summary>
/// Defina una configuración de autenticacion para los Host de interservicio.
/// </summary>
[ExcludeFromCodeCoverage]
public class AutenticacionJWT
{
    /// <summary>
    /// Clave única de la configuración.
    /// </summary>
    required public string Clave { get; set; }

    /// <summary>
    /// URl del servicio de genración de tokens.
    /// </summary>
    required public string UrlToken { get; set; }

    /// <summary>
    /// Identificador del cliente de OpenId utilziado para autenticar.
    /// </summary>
    required public string ClientId { get; set; }

    /// <summary>
    /// Scope del cliente de OpenId utilziado para autenticar.
    /// </summary>
    required public string Scope { get; set; }
}
