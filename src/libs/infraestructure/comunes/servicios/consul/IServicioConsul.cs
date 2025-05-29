namespace comunes.servicios.consul;

/// <summary>
/// Deefinicion de los servicos de consul.
/// </summary>
public interface IServicioConsul
{
    /// <summary>
    /// Obtiene la configuracion de la API desde el consul.
    /// </summary>
    /// <returns>Configuracion de la API.</returns>
    Task<ConfiguracionAPI?> ObtieneConfiguracionApi();
}
