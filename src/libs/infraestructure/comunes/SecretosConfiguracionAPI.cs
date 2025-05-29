using System.Diagnostics.CodeAnalysis;
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
namespace comunes;

/// <summary>
/// DTO de los secretos de configuracion para la API.
/// </summary>
[ExcludeFromCodeCoverage]
public class SecretosConfiguracionAPI
{
    public List<SecretoAuthConfig> Secretos { get; set; } = [];
}

/// <summary>
/// DTO de los secretos de la API.
/// </summary>
public class SecretoAuthConfig
{
    public string Clave { get; set; }
    public string Secreto { get; set; }
}
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
