using System.Diagnostics.CodeAnalysis;

namespace comunes.proxies.proxygenerico;

/// <summary>
/// Configuracion para el servicio de consul.
/// </summary>
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
[ExcludeFromCodeCoverage]
public class ConfiguracionConsul
{
    /// <summary>
    /// Url base del servicio de consul.
    /// </summary>
    public string UrlConsul { get; set; }

    /// <summary>
    /// URL base del servicio de indentidad.
    /// </summary>
    public string UrlIdentity { get; set; }
}
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
