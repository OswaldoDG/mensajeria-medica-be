using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.mensajes;
using mensajeriamedica.services.comunicaciones.helper;
using mensajeriamedica.services.comunicaciones.servicios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace mensajeriamedica.api.comunicaciones.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MensajesController(ILogger<MensajesController> logger,  IServicioMensajes servicioMensajes, IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("buscar")]
    [SwaggerOperation(Description = "Obtiene una lista de mesanejs en base a la búsqueda.", OperationId = "BuscarMensajes")]
    [SwaggerResponse(statusCode: 200, type: typeof(ResultadoPaginado<DtoMensaje>), description: "Paginado de mensajes encontrados")]
    [SwaggerResponse(statusCode: 400, description: "Datos incorrectos")]
    [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
    [SwaggerResponse(statusCode: 401, description: "No autorizado")]
    public async Task<ActionResult<ResultadoPaginado<DtoMensaje>>> BuscarMensajes([FromBody] Busqueda busqueda)
    {
        logger.LogDebug("MensajesController - BuscarMensajes {Payload}", JsonConvert.SerializeObject(busqueda));
        var respuesta = await servicioMensajes.BuscarMensajes(busqueda);

        if (!respuesta.Ok)
        {
            logger.LogDebug("MensajesController - error al buscar mensajes {Status} {Mensaje} {Codigo}", respuesta.Error!.HttpCode, respuesta.Error.Mensaje, respuesta.Error.Codigo);

            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
        }

        return Ok(respuesta.Payload);
    }

    [HttpPost("buscar-descarga")]
    public async Task<IActionResult> ExcelBusquedaMensajes([FromBody] Busqueda busqueda)
    {
        try
        {
            string directorioBase = _configuration["ExcelDir"];

            if (string.IsNullOrEmpty(directorioBase))
                return StatusCode(500, new { error = "Directorio de Excel no configurado." });

            if (!Directory.Exists(directorioBase))
            {
                Directory.CreateDirectory(directorioBase);
            }

            string nombreArchivo = $"Resultados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string rutaCompleta = Path.Combine(directorioBase, nombreArchivo);

            var respuesta = await servicioMensajes.ExcelBusquedaMensajes(busqueda, rutaCompleta);

            if (!respuesta.Ok)
            {
                logger.LogDebug("MensajesController - error al buscar mensajes {Status} {Mensaje} {Codigo}",
                    respuesta.Error?.HttpCode, respuesta.Error?.Mensaje, respuesta.Error?.Codigo);

                return ActionFromCode(respuesta.HttpCode, respuesta.Error?.Mensaje);
            }

            byte[] contenidoArchivo = await System.IO.File.ReadAllBytesAsync(rutaCompleta);

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(contenidoArchivo, mimeType, nombreArchivo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando Excel");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [NonAction]
    protected ActionResult ActionFromCode(HttpStatusCode code, string? Error = null, string? codigoError = null)
    {
        logger.LogDebug("{Code} {Mensaje}", code, Error);
        string contenido = $"{codigoError}: {Error}";
        return StatusCode(code.GetHashCode(), contenido);
    }

    [NonAction]
    protected ActionResult ActionFromCode(HttpStatusCode code, ErrorProceso? error)
    {
        logger.LogDebug("{Code} {Mensaje}", code, error?.Mensaje);
        string contenido = $"{error?.Codigo}: {error?.Mensaje}";
        return StatusCode(code.GetHashCode(), contenido);
    }
}
