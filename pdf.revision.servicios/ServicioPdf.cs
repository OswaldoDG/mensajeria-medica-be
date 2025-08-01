using comunes.respuestas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pdf.revision.model;
using pdf.revision.model.dtos;
using pdf.revision.servicios.datos;
using System.Net;

namespace pdf.revision.servicios;

public class ServicioPdf(ILogger<ServicioPdf> pdf, DbContextPdf db) : IServicioPdf
{
    public async Task<RespuestaPayload<ArchivoPdf>> PorId(int id, Guid usuarioId)
    {
        // 1. debe obtener de la base de datos el elemento con el id indicado.
        //  1.1 Si no hay elementos pendientes, debe retornar un RespuestaPayload con el valor nulo y estado 404.
        // 2. debe retornar el elemento obtenido convertido a ArchivoPdf con todos las revisiones y partes documentales y estado 200.
        var archivo = await db.Archivos.Include(a => a.Partes).Include(a => a.Revisiones).FirstOrDefaultAsync(a => a.Id == id);

        if (archivo is null)
        {
            return new RespuestaPayload<ArchivoPdf>
            {
                Payload = null,
                Error = new ErrorProceso
                {
                    Codigo = "NO_ENCONTRADO",
                    Mensaje = "Archivo no encontrado.",
                    HttpCode = HttpStatusCode.NotFound
                }
            };
        }
        return new RespuestaPayload<ArchivoPdf>
        {
            Payload = archivo,
            HttpCode = HttpStatusCode.OK
        };
    }

    public async Task<RespuestaPayload<DtoArchivo>> SiguientePendiente(Guid usuarioId)
    {
        // 1. debe obtener de la  base de datos el siguiente elemento en estado Pendiente ordenando por prioridad descencendete.
        // 1.1 Si no hay elementos pendientes debe llamar a ReiniciaPdfZombies() y volver a realizar la busqueda
        // 1.2 Si no hay elementos pendientes, debe retornar un RespuestaPayload con el valor nulo y estado 404.
        // 2. debe actualizar el estado del elemento a EnRevisionvy establecer UltimaRevision = DateTime.UtcNow.
        // 3. debe insertar un nuevo elemento  RevisionPdf con el id del usuario en sesion y el id del archivo.
        // 4. debe retornar el elemento obtenido en el paso 1 convertido a DtoArchivo y estado 200.
        var archivo = await db.Archivos.Where(a => a.Estado == EstadoRevision.Pendiente).OrderByDescending(a => a.Prioridad).FirstOrDefaultAsync();

        if (archivo is null)
        {
            await ReiniciaPdfZombies();

            archivo = await db.Archivos.Where(a => a.Estado == EstadoRevision.Pendiente).OrderByDescending(a => a.Prioridad).FirstOrDefaultAsync();

            if (archivo is null)
            {
                return new RespuestaPayload<DtoArchivo>
                {
                    Payload = null,
                    Error = new ErrorProceso
                    {
                        Codigo = "NO_PENDIENTES",
                        Mensaje = "No hay archivos pendientes para revisión.",
                        HttpCode = HttpStatusCode.NotFound
                    }
                };
            }
        }

        archivo.Estado = EstadoRevision.EnCurso;
        archivo.UltimaRevision = DateTime.UtcNow;

        var revision = new RevisionPdf
        {
            ArchivoPdfId = archivo.Id,
            UsuarioId = usuarioId,
            FechaInicioRevision = DateTime.UtcNow
        };

        db.Revisiones.Add(revision);
        await db.SaveChangesAsync();

        var dto = new DtoArchivo
        {
            Id = archivo.Id,
            Nombre = archivo.Nombre
        };

        return new RespuestaPayload<DtoArchivo>
        {
            Payload = dto,
            HttpCode = HttpStatusCode.OK
        };
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
        var archivo = await db.Archivos.Include(a => a.Partes).FirstOrDefaultAsync(a => a.Id == id);

        if (archivo is null)
        {
            return new Respuesta
            {
                Error = new ErrorProceso
                {
                    Codigo = "NO_ENCONTRADO",
                    Mensaje = "Archivo no encontrado.",
                    HttpCode = HttpStatusCode.NotFound
                }
            };
        }

        if (archivo.Estado != EstadoRevision.EnCurso)
        {
            return new Respuesta
            {
                Error = new ErrorProceso
                {
                    Codigo = "ESTADO_INVALIDO",
                    Mensaje = "El archivo no está en revisión.",
                    HttpCode = HttpStatusCode.Conflict
                }
            };
        }

        if (partes.Count == 0 || totalPaginas <= 0)
        {
            return new Respuesta
            {
                Error = new ErrorProceso
                {
                    Codigo = "DATOS_INVALIDOS",
                    Mensaje = "Debe proporcionar partes válidas y un número de páginas mayor a cero.",
                    HttpCode = HttpStatusCode.Conflict
                }
            };
        }

        archivo.Partes = partes;
        archivo.TotalPaginas = totalPaginas;

        var revision = await db.Revisiones.Where(r => r.ArchivoPdfId == id && r.UsuarioId == usuarioId).OrderByDescending(r => r.FechaInicioRevision).FirstOrDefaultAsync();

        if (revision is not null)
        {
            revision.FechaFinRevision = DateTime.UtcNow;
        }

        archivo.Estado = EstadoRevision.Finalizada;

        await db.SaveChangesAsync();

        return new Respuesta
        {
            Ok = true,
            HttpCode = HttpStatusCode.OK
        };
    }

    public async Task<Respuesta> ReiniciaPdfZombies()
    {
        // 1. Obtiene la lista de todos los archivos PDF que se encuentren en estado EnRevision y con UltimaRevision menor a DateTime.UtcNow.AddHours(-2)
        //    Es decir que fueron iniciados hace mas de 2 horas.
        // 2. Para cada ArchivoPdf revisar si hay partes documentales y de ser asi eliminarlas.
        // 3. Actualizar el estado de cada ArchivoPdf a Pendiente y UltimaRevision a null.
        var limite = DateTime.UtcNow.AddHours(-2);

        var zombies = await db.Archivos.Include(a => a.Partes).Where(a => a.Estado == EstadoRevision.EnCurso && a.UltimaRevision < limite).ToListAsync();

        foreach (var archivo in zombies)
        {
            if (archivo.Partes?.Any() == true)
            {
                db.PartesArchivo.RemoveRange(archivo.Partes);
            }

            archivo.Estado = EstadoRevision.Pendiente;
            archivo.UltimaRevision = null;
        }

        await db.SaveChangesAsync();

        return new Respuesta
        {
            Ok = true,
            HttpCode = HttpStatusCode.OK
        };
    }

    public async Task<RespuestaPayload<FileContentResult>> DescargaPdfPorId(int id)
    {
        RespuestaPayload<FileContentResult> respuesta = new RespuestaPayload<FileContentResult>();
        try
        {
            var archivo = await db.Archivos.FirstAsync(a => a.Id == id);

            if (archivo is null)
            {
                respuesta.Error = new ErrorProceso
                {
                    Codigo = "ESTADO_INVALIDO",
                    Mensaje = "El archivo no está en revisión.",
                    HttpCode = HttpStatusCode.Conflict
                };
                return respuesta;
            }

            var bytes = System.IO.File.ReadAllBytes(archivo.Ruta);
            var rutaOriginal = Path.GetFileName(archivo.Ruta);
            string directorio = Path.GetDirectoryName(rutaOriginal);
            string nombreArchivo = Path.GetFileNameWithoutExtension(rutaOriginal);
            string extension = Path.GetExtension(rutaOriginal);

            string nuevoNombre = $"{nombreArchivo}_{id}{extension}";
            string nuevaRuta = Path.Combine(directorio, nuevoNombre);

            respuesta.Payload = new FileContentResult(bytes, "application/pdf")
            {
                FileDownloadName = nuevaRuta
            };

            respuesta.Ok = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return respuesta;
    }

    public async Task<RespuestaPayload<List<DtoArchivos>>> ObtieneTipoDocumentos(DtoTipoDocumento lista)
    {
        RespuestaPayload<List<DtoArchivos>> respuesta = new RespuestaPayload<List<DtoArchivos>>();
        try
        {
            var documentos = await db.TiposDocumento
                .Include(x => x.Partes)
                .Where(p => lista.Ids.Contains(p.Id))
                .Select(td => new DtoArchivos
                {
                    Id = td.Id,
                    Nombre = td.Nombre,
                    Partes = td.Partes.Select(p => new DtoParte
                    {
                        Id = p.Id,
                        PaginaInicio = p.PaginaInicio,
                        PaginaFin = p.PaginaFin
                    }).ToList()
                })
                .ToListAsync();

            if (lista.Ids.Count == 0)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Codigo = "No hay Documentos",
                    Mensaje = "No existen documentos para obtener",
                    HttpCode = HttpStatusCode.Conflict
                };
                return respuesta;
            }

            respuesta.Ok = true;
            respuesta.Payload = documentos;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return respuesta;
    }
}
