using System.Diagnostics.CodeAnalysis;

namespace comunes;

/// <summary>
/// Define la configuración de API para las llamadas interservicio.
/// </summary>
[ExcludeFromCodeCoverage]
public class ConfiguracionAPI
{
    /// <summary>
    /// Define la clave bajo la cual se guarda la configuración de la api en ENV o Appsettings.
    /// </summary>
    public const string ClaveConfiguracionBase = "ConfiguracionAPI";

    /// <summary>
    /// DEfine la clave por defecto para el endpoint de autenticacion.
    /// </summary>
    public const string ClaveEndpointAuthDefault = "default";

    /// <summary>
    /// Lista de configuraciones de autenticacion JWT.
    /// </summary>
    public List<AutenticacionJWT> AuthConfigJWT { get; set; } = [];

    /// <summary>
    /// Lista de hosts interservicio.
    /// </summary>
    public List<HostInterServicio> Hosts { get; set; } = [];
}