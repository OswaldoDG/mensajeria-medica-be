using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.mensajes;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.servicios;

public class ServicioMensajes(ILogger<ServicioMensajes> logger, DbContextMensajeria db) : IServicioMensajes
{
    public Task<RespuestaPayload<ResultadoPaginado<DtoMensaje>>> BuscarMensajes(Busqueda busqueda)
    {
        throw new NotImplementedException();
    }
}
