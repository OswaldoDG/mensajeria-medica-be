using comunes.respuestas;
using pdf.revision.model;
using pdf.revision.model.dtos;

namespace pdf.revision.servicios;

/// <summary>
/// Interfaz para los servicios relacionados con la generacion y manejo de PDFs.
/// </summary>
public interface IServicioPdf
{
    /// <summary>
    /// Obtiene el siguiente documento PDF pendiente para separacion.
    /// </summary>
    /// <param name="usuarioId">Identificado del usuario en sesion.</param>
    /// <returns>Datos del documento o nulo si no hay pendientes.</returns>
    Task<RespuestaPayload<DtoArchivo>> SiguientePendiente(Guid usuarioId);

    /// <summary>
    /// Obtiene el siguiente documento PDF por su id..
    /// </summary>
    /// <param name="id">Id del pdf a buscar.</param>
    /// <param name="usuarioId">Identificado del usuario en sesion.</param>
    /// <returns>Datos del documento o nulo si no existe.</returns>
    Task<RespuestaPayload<ArchivoPdf>> PorId(int id, Guid usuarioId);

    /// <summary>
    /// Actualiza las partes de un documento Pdf.
    /// </summary>
    /// <param name="id">Id del pdf a buscar.</param>
    /// <param name="partes">Lista de partes que componen el documento</param>
    /// <param name="totalPaginas">Total de p[aginas del PDF</param>
    /// <param name="usuarioId">Identificado del usuario en sesion.</param>
    /// <returns>OK si el ajuste fue adecuado o false en caso contrario.</returns>
    Task<Respuesta> CreaPartesPdf(int id, List<ParteDocumental> partes, int totalPaginas, Guid usuarioId);

    /// <summary>
    /// Reinicia a estado Pendiente todos los PDFs que se encuentren en estado Zombie (EnRevision sin partes documentales en base a una fecha).
    /// </summary>
    /// <returns>Resultado de la operacion.</returns>
    Task<Respuesta> ReiniciaPdfZombies();
}
