using comunes.respuestas;
using Microsoft.Extensions.Logging;
using pdf.revision.model;
using pdf.revision.model.dtos;
using pdf.revision.servicios.datos;

namespace pdf.revision.servicios;

public class ServicioPdf(ILogger<ServicioPdf> pdf, DbContextPdf db) : IServicioPdf
{
    public async Task<RespuestaPayload<ArchivoPdf>> PorId(int id, Guid usuarioId)
    {
        // 1. debe obtener de la base de datos el elemento con el id indicado.
        //  1.1 Si no hay elementos pendientes, debe retornar un RespuestaPayload con el valor nulo y estado 404.
        // 2. debe retornar el elemento obtenido convertido a ArchivoPdf con todos las revisiones y partes documentales y estado 200.
        throw new NotImplementedException();
    }

    public async Task<RespuestaPayload<DtoArchivo>> SiguientePendiente(Guid usuarioId)
    {
        // 1. debe obtener de la  base de datos el siguiente elemento en estado Pendiente ordenando por prioridad descencendete.
        // 1.1 Si no hay elementos pendientes debe llamar a ReiniciaPdfZombies() y volver a realizar la busqueda
        // 1.2 Si no hay elementos pendientes, debe retornar un RespuestaPayload con el valor nulo y estado 404.
        // 2. debe actualizar el estado del elemento a EnRevisionvy establecer UltimaRevision = DateTime.UtcNow.
        // 3. debe insertar un nuevo elemento  RevisionPdf con el id del usuario en sesion y el id del archivo.
        // 4. debe retornar el elemento obtenido en el paso 1 convertido a DtoArchivo y estado 200.
        throw new NotImplementedException();
    }

    public async Task<Respuesta> CreaPartesPdf(int id, List<ParteDocumental> partes, int totalPaginas, Guid usuarioId)
    {
        // 1. debe obtener de la base de datos el elemento con el id indicado.
        //  1.1 Si el archivo no existe debe devolver las Respuesta con estado 404.
        //  1.2 Si el elemento no esta en estado EnRevision, debe retornar un Respuesta con estado 409.
        //  1.3 si el numero de partes es cero o si el total de paginas no es un entero positico devolver 409.
        // 2. Si esta en revision debe insertar todas las partes documentales y actualziar el TotalPaginas con el valor enviado.
        // 3. Debe obtener la revision mas reciente para el usuarioIdde RevisionPdf y actualizar FechaFinRevision DateTime.UtcNow.
        // 3. Debe actualizar el estado del elemento a Revisado y devolver 200.
        throw new NotImplementedException();
    }

    public async Task<Respuesta> ReiniciaPdfZombies()
    {
        // 1. Obtiene la lista de todos los archivos PDF que se encuentren en estado EnRevision y con UltimaRevision menor a DateTime.UtcNow.AddHours(-2)
        //    Es decir que fueron iniciados hace mas de 2 horas.
        // 2. Para cada ArchivoPdf revisar si hay partes documentales y de ser asi eliminarlas.
        // 3. Actualizar el estado de cada ArchivoPdf a Pendiente y UltimaRevision a null.
        throw new NotImplementedException();
    }
}
