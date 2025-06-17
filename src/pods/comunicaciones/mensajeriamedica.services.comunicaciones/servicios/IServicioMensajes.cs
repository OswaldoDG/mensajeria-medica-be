using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.mensajes;

namespace mensajeriamedica.services.comunicaciones.servicios;

/// <summary>
/// Servicio para la gestión de mensajes.
/// </summary>
public interface IServicioMensajes
{
    Task<RespuestaPayload<ResultadoPaginado<DtoMensaje>>> BuscarMensajes(Busqueda busqueda);
}
