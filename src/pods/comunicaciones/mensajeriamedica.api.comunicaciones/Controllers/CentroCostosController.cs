using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.centroscostos;
using mensajeriamedica.services.comunicaciones.servicios;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace mensajeriamedica.api.comunicaciones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentroCostosController(ILogger<CentroCostosController> logger, IServicioCentroCostos servicioCentroCostos) : ControllerBase
    {
        [HttpGet]
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
        public async Task<ActionResult<CentroCostos>> ActualizaCentroCostos([FromRoute] int id, [FromBody] CentroCostosDto dto)
        {
            var respuesta = await servicioCentroCostos.ActualizaCentroCostos(id, dto.Nombre);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CentroCostos>> EliminaCentroCostos([FromRoute] int id)
        {
            var respuesta = await servicioCentroCostos.EliminaCentroCostos(id);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok();
        }

        [HttpPost("{id}/unidad")]
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
        public async Task<ActionResult<UnidadCostos>> EliminaUnidadCentroCostos([FromRoute] int id, [FromRoute] int uid)
        {
            var respuesta = await servicioCentroCostos.EliminaUnidadCostos(id, uid);
            if (!respuesta.Ok)
            {

                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
            }

            return Ok();
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
