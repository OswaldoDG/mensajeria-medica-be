using mensajeriamedica.model.comunicaciones.mensajes;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.servicios;

/// <summary>
/// Servicio de gestión de envío de emnsajes.
/// </summary>
/// <param name="logger">Logger.</param>
public class ServicioEnvioWhatsApp(ILogger<ServicioEnvioWhatsApp> logger)
{
    /// <summary>
    /// Envía un mensaje de WhatsApp.
    /// </summary>
    /// <param name="mensaje">Mensaje a enviar.</param>
    public async Task EnviarMensaje(Mensaje mensaje)
    {
        throw new NotImplementedException("EnviarMensaje no está implementado en el servicio de WhatsApp.");
    }
}
