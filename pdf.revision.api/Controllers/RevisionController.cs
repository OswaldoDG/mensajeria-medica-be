using Azure.Core;
using comunes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pdf.revision.model;
using pdf.revision.model.dtos;
using pdf.revision.model.dtos.Nuevos;
using pdf.revision.servicios;

namespace pdf.revision.api.Controllers;

[Route("revision")]
[ApiController]
public class RevisionController(ILogger<RevisionController> logger, IServicioPdf servicioPdf) : ControllerUsoInterno(logger)
{

    [HttpGet("siguiente")]
    public async Task<ActionResult<DtoArchivo>> SiguientePendiente()
    {
        logger.LogInformation("Obteniendo siguiente PDF pendiente.");
        var respuesta = await servicioPdf.SiguientePendiente(UsuarioGuid!.Value);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return Ok(respuesta.Payload);
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
        var respuesta = await servicioPdf.CreaPartesPdf(id, dto.Partes, dto.TotalPaginas, UsuarioGuid!.Value);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return Ok();
    }

    [HttpGet("pdf/{id}")]
    public async Task<IActionResult> DescargaPdfPorId(int id)
    {
        logger.LogInformation("Descargando Documento PDF {Id}", id);

        var respuesta = await servicioPdf.DescargaPdfPorId(id);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return respuesta.Payload!;
    }


    [HttpPost("tiposDocumento")]
    public async Task<ActionResult<List<DtoTipoDoc>>> ObtieneTiposDocumento([FromBody]DtoTipoDocumento  lista)
    {
        logger.LogInformation("Obteniendo documentos");

        if (lista?.Ids == null || lista.Ids.Count == 0)
            return BadRequest("La lista de IDs está vacía.");

        var respuesta = await servicioPdf.ObtieneTipoDocumentos();

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Codigo);
        }

        return respuesta.Payload!;
    }
}
