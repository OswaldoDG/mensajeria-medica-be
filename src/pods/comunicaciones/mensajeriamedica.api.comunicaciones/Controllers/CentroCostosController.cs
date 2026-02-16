using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.centroscostos;
using mensajeriamedica.model.comunicaciones.centroscostos.dtos;
using mensajeriamedica.model.comunicaciones.mensajes;
using mensajeriamedica.services.comunicaciones.servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace mensajeriamedica.api.comunicaciones.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CentroCostosController(ILogger<CentroCostosController> logger, IServicioCentroCostos servicioCentroCostos) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Description = "Obtiene una lista de Centros de Costos.", OperationId = "ObtieneCentroCostos")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<CentroCostos>), description: "Lista Centros de Costos")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult<List<CentroCostos>>> ObtieneCentroCostos([FromQuery(Name = "eliminados")] bool? eliminados = false)
        {
            var respuesta = await servicioCentroCostos.ObtieneCentroCostos(eliminados.Value);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok(respuesta.Payload);
        }

        [HttpPost]
        [SwaggerOperation(Description = "Crea un centro de costos.", OperationId = "CreaCentroCostos")]
        [SwaggerResponse(statusCode: 200, type: typeof(CentroCostos), description: "Centro de Costos Creado")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult<CentroCostos>> CreaCentroCostos([FromBody] CentroCostosDto dto)
        {
            var respuesta = await servicioCentroCostos.CreaCentroCostos(dto.Nombre);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok(respuesta.Payload);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Description = "Actualiza un Centro de Costos.", OperationId = "CreaCentroCostos")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult> ActualizaCentroCostos([FromRoute] int id, [FromBody] CentroCostosDto dto)
        {
            var respuesta = await servicioCentroCostos.ActualizaCentroCostos(id, dto.Nombre);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Description = "Elimina un centro de costos.", OperationId = "EliminaCentroCostos")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult> EliminaCentroCostos([FromRoute] int id)
        {
            var respuesta = await servicioCentroCostos.EliminaCentroCostos(id);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok();
        }

        [HttpPost("{id}/unidad")]
        [SwaggerOperation(Description = "Crea Unidad de Costos.", OperationId = "CreaUnidadCentroCostos")]
        [SwaggerResponse(statusCode: 200, type: typeof(UnidadCostos), description: "Unidad de Costos Creada")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult<UnidadCostos>> CreaUnidadCentroCostos([FromRoute] int id, [FromBody] UnidadCostosDto dto)
        {
            var respuesta = await servicioCentroCostos.CreaUnidadCostos(id, dto.Nombre, dto.Clave);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok(respuesta.Payload);
        }

        [HttpPost("{id}/unidad/{uid}")]
        [SwaggerOperation(Description = "Actualiza Unidad de Costos.", OperationId = "ActualizaUnidadCentroCostos")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult<UnidadCostos>> ActualizaUnidadCentroCostos([FromRoute] int id, [FromRoute] int uid, [FromBody] UnidadCostosDto dto)
        {
            var respuesta = await servicioCentroCostos.ActualizaUnidadCostos(id, uid, dto.Nombre, dto.Clave);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok();
        }

        [HttpDelete("{id}/unidad/{uid}")]
        [SwaggerOperation(Description = "Elimina Unidad de Costos.", OperationId = "EliminaUnidadCentroCostos")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult<UnidadCostos>> EliminaUnidadCentroCostos([FromRoute] int id, [FromRoute] int uid)
        {
            var respuesta = await servicioCentroCostos.EliminaUnidadCostos(id, uid);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok();
        }

        [HttpGet("obtiene/usuarios")]
        [SwaggerOperation(Description = "Obtiene Lista de Usuarios", OperationId = "ObtieneListaUsuarios")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<CentroCostos>), description: "Lista Centros de Costos")]
        [SwaggerResponse(statusCode: 409, description: "Datos incorrectos")]
        [SwaggerResponse(statusCode: 403, description: "Sin acceso")]
        [SwaggerResponse(statusCode: 401, description: "No autorizado")]
        public async Task<ActionResult<List<DtoUsuario>>> ObtieneListaUsuarios()
        {
            var respuesta = await servicioCentroCostos.ObtieneListaUsuarios();
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok(respuesta.Payload);
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
}
