namespace comunes;

/// <summary>
/// Define uns host de llamadas interservicio
/// </summary>
public class HostInterServicio
{
    /// <summary>
    /// Clave única del servicio
    /// </summary>
    public required string Clave { get; set; }

    /// <summary>
    /// Especifica el tipo de autenticacion
    /// </summary>
    public TipoAutenticacion TipoAutenticacion { get; set; } = TipoAutenticacion.Ninguna;

    /// <summary>
    /// Url base del servicio
    /// </summary>
    public required string UrlBase { get; set; }

    /// <summary>
    /// Define la clave para localizar la configuración para realizar la autenticación
    /// </summary>
    public string? ClaveAutenticacion { get; set; }
}
