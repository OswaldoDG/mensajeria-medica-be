using comunes.respuestas;

namespace comunes.proxies.proxygenerico;

/// <summary>
/// Definición del proxy genérico interservicio.
/// El proxy utiliza HTTP para comunicarse y se autentica solicitando un token JWT del servicio de identidad en base ala configuración.
/// </summary>
public interface IProxyGenericoInterservicio
{
    Task<Respuesta> JsonPost(string nombreHostServicio, string endpoint, string descripcionOperacion, object? payload);
    Task<RespuestaPayloadJson> JsonRespuestaSerializada(string nombreHostServicio, string endpoint, string descripcionOperacion, VerboHttp verbo, object? payload);
}
