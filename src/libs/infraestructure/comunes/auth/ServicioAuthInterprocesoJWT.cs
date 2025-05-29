using comunes.autenticacion.abstraccion;
using comunes.servicios.consul;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace comunes.autenticacion;

public class ServicioAuthInterprocesoJWT(ILogger<ServicioAuthInterprocesoJWT> logger,
    IServicioConsul servicioConsul, IHttpClientFactory clientFactory,
    IOptions<SecretosConfiguracionAPI> optionsSecretosApi, IDistributedCache cache) : IServicioAutenticacionJWT
{
    private readonly SecretosConfiguracionAPI secretosApi = optionsSecretosApi.Value;

    /// <summary>
    /// OBtiene un token de interproceso, si no se envía la clave de configuración toma la definida por 'auth_default'.
    /// </summary>
    /// <param name="claveConfiguracion">Clave de configuracion obtener el token.</param>
    /// <returns>Token de JWT.</returns>
    public async Task<TokenJWT?> TokenInterproceso(string claveConfiguracion = "auth_default")
    {
        try
        {
            ConfiguracionAPI? configuracionAPI = await servicioConsul.ObtieneConfiguracionApi();

            if (configuracionAPI == null || !secretosApi.Secretos.Any(s => s.Clave == claveConfiguracion))
            {
                throw new Exception("La configuración de autenticación no es válida");
            }

            var token = new TokenJWT();
            string secreto = secretosApi.Secretos.First(s => s.Clave == claveConfiguracion).Secreto;
            string? tokenCache = await cache.GetStringAsync(claveConfiguracion);

            if (tokenCache == null)
            {
                var authConfigJWT = configuracionAPI.AuthConfigJWT.FirstOrDefault(_ => _.Clave == claveConfiguracion);
                if (authConfigJWT != null)
                {
                    Dictionary<string, string> dict = new ()
                            {
                                {"grant_type", "client_credentials"},
                                { "client_id", authConfigJWT.ClientId },
                                { "client_secret", secreto }
                            };

                    var uri = new Uri($"{authConfigJWT.UrlToken}/connect/token");
                    logger.LogDebug("Conectando a {Uri} con cliente {Cliente}/{Secreto}", uri.ToString(), authConfigJWT.ClientId, secreto[..3]);

                    HttpClient _client = clientFactory.CreateClient("token");
                    var result = await _client.PostAsync(uri, new FormUrlEncodedContent(dict));
                    string payload = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode)
                    {
                        token = JsonConvert.DeserializeObject<TokenJWT>(payload);
                        var options = new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token!.expires_in * .9)
                        };

                        await cache.SetStringAsync(claveConfiguracion, payload, options);
                    }
                    else
                    {
                        logger.LogError("Error al obtener el token de acceso {Codigo}, {Motivo}, {Payload}", result.StatusCode, result.ReasonPhrase, payload);
                    }
                } else
                {
                    logger.LogError("No existe la configuración de identidad para {Clave}", claveConfiguracion);
                }
            }
            else
            {
                token = JsonConvert.DeserializeObject<TokenJWT>(tokenCache);
            }

            return token;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al  generar el token para {Clave}", claveConfiguracion);
            return null;
        }

    }
}
