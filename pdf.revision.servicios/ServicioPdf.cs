using System.Net;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using comunes.respuestas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using pdf.revision.model;
using pdf.revision.model.dtos;
using pdf.revision.model.dtos.Nuevos;
using pdf.revision.servicios.datos;
using Polly;

namespace pdf.revision.servicios;

public class ServicioPdf(ILogger<ServicioPdf> pdf, DbContextPdf db, IConfiguration configuration, IDistributedCache cache) : IServicioPdf
{
    private readonly string connectionString = configuration!.GetValue<string>("azure:blob:connectionString")!;
    private readonly string containerpdfs = configuration!.GetValue<string>("azure:blob:containerpdfs")!;
    private readonly string folderpdfs = configuration!.GetValue<string>("azure:blob:folderpdfs")!;
    private readonly int duracionMinutosSasToken = configuration!.GetValue<int>("azure:blob:duracionMinutosSasToken")!;
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


        await ReiniciaPdfZombies();
        var archivo = await db.Archivos.Where(a => a.Estado == EstadoRevision.Pendiente).OrderByDescending(a => a.Prioridad).FirstOrDefaultAsync();

        if (archivo is null)
        {
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
            Nombre = archivo.Nombre,
            TokenSAS = GeneraTokenSAS(archivo.Nombre),
        };

        return new RespuestaPayload<DtoArchivo>
        {
            Payload = dto,
            HttpCode = HttpStatusCode.OK
        };
    }

    public async Task<Respuesta> CreaPartesPdf(int id, List<DtoParteDocumental> partes, int totalPaginas, Guid usuarioId)
    {
        var archivo = await db.Archivos
            .Include(a => a.Partes)
            .FirstOrDefaultAsync(a => a.Id == id);

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

        if (partes.Count > 0 )
        {
            if (archivo.Partes == null)
                archivo.Partes = new List<ParteDocumental>();

            foreach (var dtoParte in partes)
            {
                var parte = new ParteDocumental
                {
                    ArchivoPdfId = archivo.Id,
                    PaginaInicio = dtoParte.PaginaInicio,
                    PaginaFin = dtoParte.PaginaFin,
                    TipoDocumentoId = dtoParte.TipoDocumentoId,
                    IdAgrupamiento = dtoParte.IdAgrupamiento
                };

                archivo.Partes.Add(parte);
            }
        }

        archivo.TotalPaginas = totalPaginas;
        archivo.Estado = EstadoRevision.Finalizada;

        var revision = await db.Revisiones
            .Where(r => r.ArchivoPdfId == id && r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.FechaInicioRevision)
            .FirstOrDefaultAsync();

        if (revision != null)
        {
            revision.FechaFinRevision = DateTime.UtcNow;
            db.Revisiones.Update(revision);
        }

        db.Archivos.Update(archivo);

        var cambios = await db.SaveChangesAsync();
        Console.WriteLine($"Cambios realizados: {cambios}");

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

    public async Task<RespuestaPayload<DtoArchivo>> SiguientePorId(int id, Guid usuarioId)
    {
        var archivo = await db.Archivos.Where(a => a.Estado == EstadoRevision.Pendiente && a.Id == id).OrderByDescending(a => a.Prioridad).FirstOrDefaultAsync();

        if (archivo is null)
        {
            await ReiniciaPdfZombies();

            archivo = await db.Archivos.Where(a => a.Estado == EstadoRevision.Pendiente && a.Id == id).OrderByDescending(a => a.Prioridad).FirstOrDefaultAsync();

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
            Nombre = archivo.Nombre,
            TokenSAS = GeneraTokenSAS(archivo.Nombre),
        };

        return new RespuestaPayload<DtoArchivo>
        {
            Payload = dto,
            HttpCode = HttpStatusCode.OK
        };
    }


    /// <summary>
    /// Obtiene la lista de documentos disponibles para la tipificacion.
    /// </summary>
    /// <returns>Lista de tipos de documento</returns>
    public async Task<RespuestaPayload<List<DtoTipoDoc>>> ObtieneTipoDocumentos()
    {
        RespuestaPayload<List<DtoTipoDoc>> respuesta = new RespuestaPayload<List<DtoTipoDoc>>();
        try
        {
            var documentos = await db.TiposDocumento.ToListAsync();
            var lista = documentos.Select(d => new DtoTipoDoc() { Nombre = d.Nombre, Id = d.Id , Tecla = d.Tecla }).OrderBy(d => d.Nombre).ToList();
            respuesta.Ok = true;
            respuesta.Payload = lista;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return respuesta;
    }

    public async Task<Respuesta> PdfsBlobToDataBase(string nombreBlob)
    {
        var respuesta = new Respuesta();
        try
        {
            BlobServiceClient _blobServiceClient = CrearBlobServiceClientDesdeConfiguracion();

            var containerClient = CrearContainerDesdeConfiguracion(_blobServiceClient);

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix:nombreBlob))
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);

                var archivo = new ArchivoPdf
                {
                    Nombre = Path.GetFileName(blobItem.Name),
                    Ruta = blobClient.Uri.ToString(),
                    Estado = EstadoRevision.Pendiente,
                    UltimaRevision = null,
                    TotalPaginas = 0,
                    Prioridad = 1
                };

                bool yaExiste = await db.Archivos
                    .AnyAsync(a => a.Nombre == Path.GetFileName(blobItem.Name));

                if (!yaExiste)
                {
                    db.Archivos.Add(archivo);
                }

                await db.SaveChangesAsync();

                respuesta.Ok = true;
                respuesta.HttpCode = HttpStatusCode.OK;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return respuesta;
    }

    private BlobContainerClient CrearContainerDesdeConfiguracion(BlobServiceClient blobServiceClient)
    {
        if (string.IsNullOrEmpty(containerpdfs))
        {
            throw new InvalidOperationException("No se encontró el nombre del contenedor para Azure Blob Storage en la configuración");
        }

        return blobServiceClient.GetBlobContainerClient(containerpdfs);
    }

    private BlobServiceClient CrearBlobServiceClientDesdeConfiguracion()
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("No se encontró la cadena de conexión para Azure Blob Storage en la configuración.");
        }

        return new BlobServiceClient(connectionString);
    }

    private string GeneraTokenSAS(string nombreArchivo)
    {
        string nombreCompletoBlob = Path.Combine(folderpdfs, nombreArchivo).Replace("\\", "/");

        BlobServiceClient _blobServiceClient = CrearBlobServiceClientDesdeConfiguracion();

        var containerClient = CrearContainerDesdeConfiguracion(_blobServiceClient);

        BlobClient blobClient = containerClient.GetBlobClient(nombreCompletoBlob);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException("No se puede generar el token SAS. Verifica las credenciales.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(duracionMinutosSasToken)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

        return sasUri.ToString();
    }

    public async Task<List<DtoEstadisticasUsuarioDate>> ObtieneEstadisticasUsuario(Guid id)
    {
        List<DtoEstadisticasUsuarioDate> lista = [];
        string? cached = await cache.GetStringAsync(id.ToString());

        if (!string.IsNullOrEmpty(cached) && cached != "[]")
        {
            lista = JsonSerializer.Deserialize<List<DtoEstadisticasUsuarioDate>>(cached)!;
        }
        else
        {
            string sql = @"select DATE_FORMAT(CONVERT_TZ(r.FechaInicioRevision, '+00:00', '-06:00'), '%d-%m-%Y') as Fecha, count(r.usuarioid) as Conteo
from pdfsplit.revision_pdf r 
join aspnetusers u on r.UsuarioId = u.Id
where
r.UsuarioId = '-UID-' and r.FechaFinRevision is not null and
r.FechaInicioRevision BETWEEN UTC_TIMESTAMP() - INTERVAL 16 DAY AND UTC_TIMESTAMP()
group by DATE_FORMAT(CONVERT_TZ(r.FechaInicioRevision, '+00:00', '-06:00'), '%d-%m-%Y')
order by DATE_FORMAT(CONVERT_TZ(r.FechaInicioRevision, '+00:00', '-06:00'), '%d-%m-%Y')";

           // string baseSQl = "SELECT r.FechaFinRevision, r.ArchivoPdfId FROM pdfsplit.revision_pdf r" +
           //" inner join pdfsplit.archivo_pdf a on r.ArchivoPdfId = a.Id where r.FechaFinRevision is not null " +
           //" and (a.Estado = 4 or a.Estado = 2 ) and r.UsuarioId = '-UID-' and CONVERT_TZ(r.FechaFinRevision, '+00:00', '-06:00') " +
           //" BETWEEN UTC_TIMESTAMP() - INTERVAL 7 DAY AND UTC_TIMESTAMP();";

            string query = sql.Replace("-UID-", id.ToString());

           var  rows = await db.Set<DtoEstadisticasUsuario>().FromSqlRaw(query).ToListAsync();
            TimeZoneInfo gmtMinus6 = TimeZoneInfo.CreateCustomTimeZone("GMT-6", TimeSpan.FromHours(-6), "GMT-6", "GMT-6");

            foreach (var item in rows)
            {
                lista.Add(new DtoEstadisticasUsuarioDate
                {
                    Fecha = DateTime.ParseExact(item.Fecha, "dd-MM-yyyy", null),
                    Conteo = item.Conteo
                });
            }

            //foreach (var item in allRows)
            //{
            //    item.FechaFinRevision = TimeZoneInfo.ConvertTimeFromUtc(item.FechaFinRevision, gmtMinus6).Date;
            //}

            //var groupedCounts = allRows.GroupBy(r => r.FechaFinRevision.Date)
            //        .Select(g => new
            //        {
            //            Date = g.Key,
            //            Count = g.Count()
            //        })
            //        .OrderBy(g => g.Date)
            //        .ToList();

            //foreach (var i in groupedCounts)
            //{
            //    lista.Add(new DtoEstadisticasUsuario
            //    {
            //        Fecha = i.Date,
            //        Conteo = i.Count
            //    });
            //}

            await cache.SetStringAsync(id.ToString(), JsonSerializer.Serialize(lista), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        return lista;
    }
}
