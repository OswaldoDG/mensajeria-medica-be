using comunes.extensiones;
using comunes.proxies.proxygenerico;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace comunes.servicios.consul;

/// <summary>
/// Servicio de procesamiento de consul.
/// </summary>
/// <param name="logger">Logger.</param>
public class ServicioConsul(ILogger<ServicioConsul> logger, IHttpClientFactory httpClientFactory, IOptions<ConfiguracionConsul> optionsConsul, IDistributedCache cache) : IServicioConsul
{
    private readonly ConfiguracionConsul configuracionConsul = optionsConsul.Value;

    /// <summary>
    /// Obtiene la definición de servicios desde el consul.
    /// </summary>
    /// <returns>Definición de servicios o nulo</returns>
    public async Task<ConfiguracionAPI?> ObtieneConfiguracionApi()
    {
        var claveCache = "ConfiguracionAPI";
        var config = cache.LeeSerializado<ConfiguracionAPI>(claveCache);
        try
        {
            if (config == null)
            {
                var url = configuracionConsul.UrlConsul.TrimEnd('/') + "/consul/configuracion/api";
                logger.LogDebug("Obteniendo configuracion de consul {Url}.", url);
                var httpClient = httpClientFactory.CreateClient("consul");
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    config = JsonConvert.DeserializeObject<ConfiguracionAPI>(contenido);
                    cache.AlmacenaSerializadoAbsoluto(claveCache, config!, 15);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ProxyGenericoInterservicio - ObtieneConfiguracionApi error {Mensaje}", ex.Message);
        }

        return config;
    }
}
