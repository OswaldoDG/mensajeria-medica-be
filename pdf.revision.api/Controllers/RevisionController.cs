using comunes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pdf.revision.model.dtos;
using pdf.revision.model.dtos.Nuevos;
using pdf.revision.servicios;

namespace pdf.revision.api.Controllers;

[Route("revision")]
[Authorize]
[ApiController]
public class RevisionController(ILogger<RevisionController> logger, IServicioPdf servicioPdf) : ControllerUsoInterno(logger)
{

    [HttpGet("misestadisticas")]
    [AllowAnonymous]
    public async Task<ActionResult<List<DtoEstadisticasUsuarioDate>>> MisEstadisticas()
    {
        logger.LogInformation("Obteniendo estadisticas del usuario.");
        var respuesta = await servicioPdf.ObtieneEstadisticasUsuario(UsuarioGuid!.Value);

        return Ok(respuesta);
    }

    [HttpGet("siguiente")]
    public async Task<ActionResult<DtoArchivo>> SiguientePendiente([FromQuery(Name = "id")] string? Id )
    {
        logger.LogInformation("Obteniendo siguiente PDF pendiente.");

        bool buscando = true;
        DtoArchivo archivo = null;

        while (buscando)
        {
            var respuesta = await servicioPdf.SiguientePendiente(UsuarioGuid!.Value);
            if (respuesta.Ok)
            {
                var actual = await servicioPdf.PorId(respuesta.Payload!.Id, UsuarioGuid!.Value);
                if (actual.Payload!.UsuarioId == UsuarioGuid!.Value)
                {
                    archivo = respuesta.Payload;
                    break;
                }
            }
            else
            {
                return ActionFromCode(respuesta!.HttpCode, respuesta.Error!);
            }

        }

        return Ok(archivo);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DtoArchivo>> PdfPorId(int id)
    {
        logger.LogInformation("Obteniendo el PDF {Id}", id);
        var respuesta = await servicioPdf.PorId(id, UsuarioGuid!.Value);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return Ok(respuesta.Payload);
    }

    [HttpPost("finalizar/{id}")]
    public async Task<ActionResult> FinalizarPorId(int id, [FromBody] DtoFinalizar dto)
    {
        logger.LogInformation("Obteniendo el PDF {Id}", id);
        var respuesta = await servicioPdf.CreaPartesPdf(id, dto, UsuarioGuid!.Value);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return Ok();
    }

    [HttpGet("pdf/{id}")]
    public async Task<IActionResult> SiguientePorId(int id)
    {
        logger.LogInformation("Descargando Documento PDF {Id}", id);

        var respuesta = await servicioPdf.SiguientePorId(id, UsuarioGuid!.Value);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return Ok(respuesta.Payload);
    }

    [HttpGet("tiposDocumento")]
    public async Task<ActionResult<List<DtoTipoDoc>>> ObtieneTiposDocumento()
    {
        logger.LogInformation("Obteniendo documentos");

        var respuesta = await servicioPdf.ObtieneTipoDocumentos();

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return respuesta.Payload!;
    }

    [HttpPost("documentos-blob/{nombreFolder}")]
    public async Task<IActionResult> PdfsBlobToDataBase([FromRoute] string nombreFolder)
    {
        var respuesta = await servicioPdf.PdfsBlobToDataBase(nombreFolder);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return Ok(respuesta);
    }

    [HttpGet("validacion/{id}")]
    public async Task<IActionResult> ValidacionAsignacionAsync(int id)
    {
        var respuesta = await servicioPdf.ValidarAsigacionAsync(id, UsuarioGuid!.Value);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return Ok(respuesta);
    }
}
