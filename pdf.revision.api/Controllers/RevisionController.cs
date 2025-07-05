using comunes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pdf.revision.model.dtos;
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
    public async Task<ActionResult<DtoArchivo>> FinalizarPorId(int id, [FromBody] DtoFinalizar dto)
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
    public async Task<IActionResult> DocumentoPdfPorId(int id)
    {
        // POR DEFINIR.

        //// Ruta física del archivo PDF
        //var rutaArchivo = Path.Combine("Archivos", $"{nombreArchivo}.pdf");

        //if (!System.IO.File.Exists(rutaArchivo))
        //    return NotFound("Archivo no encontrado.");

        //var stream = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read);
        //var mimeType = "application/pdf";
        //var nombreDescarga = $"{nombreArchivo}.pdf";

        //// Devuelve el archivo como un blob con el tipo MIME de PDF
        //return File(stream, mimeType, nombreDescarga);

        throw new NotImplementedException();
    }
}
