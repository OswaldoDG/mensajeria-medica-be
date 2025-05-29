using System.Net;
using System.Net.Http.Headers;
using System.Text;
using comunes.autenticacion.abstraccion;
using comunes.extensiones;
using comunes.respuestas;
using comunes.servicios.consul;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace comunes.proxies.proxygenerico;

/// <summary>
/// Verbos Http disponibles para el proxy genérico.
/// </summary>
public enum VerboHttp
{
    POST = 0,
    GET = 1,
}

/// <summary>
/// Define un proxy de http genérico para los llamados interservicio.
/// </summary>
#pragma warning disable S2629 // Logging templates should be constant
public class ProxyGenericoInterservicio(ILogger<ProxyGenericoInterservicio> logger, IServicioAutenticacionJWT servicioAuthJWT, 
    IHttpClientFactory httpClientFactory, IServicioConsul servicioConsul) : IProxyGenericoInterservicio
{
    /// <summary>
    /// Verifica la condiguraion interservicio.
    /// </summary>
    /// <param name="nombreHostServicio">Nombr del servicio buscado.</param>
    /// <returns>Configuracion y error.</returns>
    private async Task<(HostInterServicio? host, ErrorProceso? error)> ConfiguraApi(string nombreHostServicio)
    {
        var configuracionAPI = await servicioConsul.ObtieneConfiguracionApi();
        if (configuracionAPI == null)
        {
            return (null, new ErrorProceso() { Mensaje = $"ProxyConversacionComunicaciones - error de configuracion de la API ", Codigo = "", HttpCode = HttpStatusCode.InternalServerError });
        }

        var host = configuracionAPI.ObtieneHost(nombreHostServicio);

        if (host == null || string.IsNullOrEmpty(host.ClaveAutenticacion))
        {
            return (null, new ErrorProceso() { Mensaje = $"ProxyConversacionComunicaciones - Host {nombreHostServicio} no configurado", Codigo = "", HttpCode = HttpStatusCode.UnprocessableEntity });
        }

        return (host, null);
    }

    public async Task<Respuesta> JsonPost(string nombreHostServicio, string endpoint, string descripcionOperacion, object? payload)
    {
        Respuesta respuesta = new Respuesta();
        try
        {
            logger.LogDebug($"ProxyGenericoInterservicio - JsonPost {descripcionOperacion}");
            var (host, error) = await ConfiguraApi(nombreHostServicio);
            if (error != null)
            {
                respuesta.Error = error;
            }

            HttpClient httpClient = httpClientFactory.CreateClient(nombreHostServicio);
            endpoint = $"/{endpoint.TrimStart('/')}";
            httpClient.BaseAddress = new Uri(host!.UrlBase.TrimEnd('/'));

            TokenJWT? jWT = await servicioAuthJWT!.TokenInterproceso(host.ClaveAutenticacion!);
            if (jWT == null)
            {
                logger.LogDebug($"ProxyGenericoInterservicio - JsonPost Error al obtener el token interservicio de JWT para {descripcionOperacion}");
                respuesta.Error = new ErrorProceso() { Mensaje = $"ProxyGenericoInterservicio - no fue posible obtener JWT", Codigo = "", HttpCode = HttpStatusCode.UnprocessableEntity };
            }
            else
            {
                logger.LogDebug($"ProxyGenericoInterservicio - JsonPost Llamado remoto a {httpClient.BaseAddress}/{endpoint}");

                StringContent? contenido = payload != null ? new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json") : null;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jWT.access_token);
                var response = await httpClient.PostAsync(endpoint, contenido);

                logger.LogDebug("ProxyGenericoInterservicio - JsonPost Respuesta {Code} {Reason}", response.StatusCode, response.ReasonPhrase);

                if (response.IsSuccessStatusCode)
                {
                    respuesta.Ok = true;
                }
                else
                {
                    string? contenidoRespuesta = await response.Content.ReadAsStringAsync();
                    respuesta.Error = new ErrorProceso() { Mensaje = $"ProxyGenericoInterservicio - JsonPost error llamada remota {response.ReasonPhrase} {contenidoRespuesta}", Codigo = "", HttpCode = response.StatusCode };
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ProxyGenericoInterservicio - JsonPost Error en {Servicio} {Operacion} {Mensaje}", nombreHostServicio, descripcionOperacion, ex.Message);
            respuesta.Error = new ErrorProceso() { Mensaje = $"{ex.Message}", Codigo = "", HttpCode = HttpStatusCode.InternalServerError };
        }

        return respuesta;
    }

    public async Task<RespuestaPayloadJson> JsonRespuestaSerializada(string nombreHostServicio, string endpoint, string descripcionOperacion, VerboHttp verbo, object? payload)
    {
        RespuestaPayloadJson respuesta = new RespuestaPayloadJson();
        try
        {
            var (host, error) = await ConfiguraApi(nombreHostServicio);
            if (error != null)
            {
                respuesta.Error = error;
            }

            HttpClient httpClient = httpClientFactory.CreateClient(nombreHostServicio);
            logger.LogDebug($"ProxyGenericoInterservicio - JsonRespuestaSerializada {descripcionOperacion}");
            endpoint = $"/{endpoint.TrimStart('/')}";
            httpClient.BaseAddress = new Uri(host!.UrlBase.TrimEnd('/'));

            TokenJWT? jWT = await servicioAuthJWT!.TokenInterproceso(host.ClaveAutenticacion!);
            if (jWT == null)
            {
                logger.LogDebug($"ProxyGenericoInterservicio - JsonRespuestaSerializada Error al obtener el token interservicio de JWT para {descripcionOperacion}");
                respuesta.Error = new ErrorProceso() { Mensaje = $"ProxyGenericoInterservicio - no fue posible obtener JWT", Codigo = "", HttpCode = HttpStatusCode.UnprocessableEntity };
            }
            else
            {
                logger.LogDebug($"ProxyGenericoInterservicio - JsonRespuestaSerializada Llamado remoto a {httpClient.BaseAddress}/{endpoint}");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jWT.access_token);

                HttpResponseMessage? response = null;
                switch (verbo)
                {
                    case VerboHttp.POST:
                        StringContent? contenido = payload != null ? new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json") : null;
                        response = await httpClient.PostAsync(endpoint, contenido);
                        break;

                    case VerboHttp.GET:
                        response = await httpClient.GetAsync(endpoint);
                        break;
                }

                logger.LogDebug("ProxyGenericoInterservicio - JsonRespuestaSerializada Respuesta {Code} {Reason}", response.StatusCode, response.ReasonPhrase);
                string? contenidoRespuesta = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    respuesta.Payload = contenidoRespuesta;
                }
                else
                {
                    respuesta.Error = new ErrorProceso() { Mensaje = $"ProxyGenericoInterservicio - JsonRespuestaSerializada error llamada remota {response.ReasonPhrase} {contenidoRespuesta}", Codigo = "", HttpCode = response.StatusCode };
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ProxyGenericoInterservicio - JsonRespuestaSerializada Error en {Servicio} {Operacion} {Mensaje}", nombreHostServicio, descripcionOperacion, ex.Message);
            respuesta.Error = new ErrorProceso() { Mensaje = $"{ex.Message}", Codigo = "", HttpCode = HttpStatusCode.InternalServerError };
        }

        return respuesta;
    }

}
#pragma warning restore S2629 // Logging templates should be constant