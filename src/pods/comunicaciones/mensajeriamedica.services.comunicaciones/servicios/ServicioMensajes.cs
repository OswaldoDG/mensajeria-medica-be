using comunes.busqueda;
using comunes.extensiones;
using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.mensajes;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.servicios;

public class ServicioMensajes(ILogger<ServicioMensajes> logger, DbContextMensajeria db) : IServicioMensajes
{
    public async Task<RespuestaPayload<ResultadoPaginado<DtoMensaje>>> BuscarMensajes(Busqueda busqueda)
    {
        logger.LogDebug("ServicioMensajes-BuscarMensajes {Busqueda}", busqueda);
        RespuestaPayload<ResultadoPaginado<DtoMensaje>> respuesta = new ();
        try
        {
            ServicioBusquedaSQL<Mensaje> servicio = new ServicioBusquedaSQL<Mensaje>(DbContextMensajeria.TABLA_MENSAJES);

            var pagina = await servicio.Buscar(busqueda, db);

            ResultadoPaginado<DtoMensaje> resultado = new ResultadoPaginado<DtoMensaje>()
            {
                Contar = busqueda.Contar,
                Filtros = busqueda.Filtros,
                OrdenarPropiedad = busqueda.OrdenarPropiedad,
                OrdernarDesc = busqueda.OrdernarDesc,
                Total = pagina.Total,
                Paginado = busqueda.Paginado,
                Elementos = []
            };

            foreach (var mensaje in pagina.Elementos)
            {
                var dtoMensaje = new DtoMensaje()
                {
                    Id = mensaje.Id,
                    FechaCreacion = mensaje.FechaCreacion,
                    Estado = mensaje.Estado,
                    Telefono = mensaje.Telefono,
                    NombreContacto = mensaje.NombreContacto,
                    Url = mensaje.Url,
                    ServidorId = mensaje.ServidorId,
                    SucursalId = mensaje.SucursalId
                };
                resultado.Elementos.Add(dtoMensaje);
            }
            respuesta.Payload = resultado;
            respuesta.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioMensajes-BuscarMensajes Error al realizar la búsqueda {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }
}
