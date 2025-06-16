using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using mensajeriamedica.api.comunicaciones.extensiones;
using mensajeriamedica.model.comunicaciones.mensajes;
using mensajeriamedica.model.comunicaciones.servidores;
using mensajeriamedica.services.comunicaciones.interpretes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.servicios;

/// <summary>
/// Servicio primario para la creacion de mensajes.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProcesadorArchivos(ILogger<ProcesadorArchivos> logger, IInterpreteHL7 interprete, IServiceScopeFactory scopeFactory, IConfiguration configuration, IDistributedCache cache) : BackgroundService
{
    private readonly int intervaloPoll = configuration.GetValue<int>("IntervaloSegundosPollFtp") * 1000;
    private const string CacheKey = "ClientesMensajeria";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContextMensajeria>();

                logger.LogDebug("Iniciando procesamiento de archivos...");

                var clientes = await ObtenerClientes(dbContext, stoppingToken);

                if (clientes == null || clientes.Count == 0)
                {
                    logger.LogDebug("No se encontraron clientes. Esperando el próximo ciclo...");
                    await Task.Delay(intervaloPoll, stoppingToken);
                    continue;
                }

                logger.LogDebug("Clientes válidos encontrados: {Clientes}", clientes.Count);

                foreach (var cliente in clientes)
                {
                    logger.LogDebug("Procesando cliente: {Id}", cliente.Id);
                    await ProcesarArchivosCliente(cliente, dbContext, stoppingToken);
                }

                await Task.Delay(intervaloPoll, stoppingToken);
            }
        }
    }

    private async Task<List<Servidor>> ObtenerClientes(DbContextMensajeria dbContext, CancellationToken cancellationToken)
    {
        var cacheD = await cache.GetStringAsync(CacheKey, cancellationToken);
        List<Servidor> clientes;
        var clientesValidos = new List<Servidor>();
        try
        {
            if (cacheD != null)
            {
                logger.LogDebug("Recuperando clientes desde caché");
                clientes = JsonSerializer.Deserialize<List<Servidor>>(cacheD);
            }
            else
            {
                logger.LogInformation("Consultando Clientes desde base de datos");
                clientes = await dbContext.Servidores.ToListAsync(cancellationToken);
                if (clientes.Count == 0) return clientes;
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                await cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(clientes), options, cancellationToken);
            }

            foreach (var cliente in clientes)
            {
                logger.LogDebug("Validando carpeta para cliente {Id}: {FolderFtp}", cliente.Id, cliente.FolderFtp);

                if (!Directory.Exists(cliente.FolderFtp))
                {
                    logger.LogError("No existe el directorio {Folder}", cliente.FolderFtp);
                    dbContext.Servidores.Remove(cliente);
                    await dbContext.SaveChangesAsync();
                    clientes.Remove(cliente);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    await cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(clientes), options, cancellationToken);
                    continue;
                }

                var subDirs = new[] { "ok", "erroneos", "duplicados" };

                foreach (var dir in subDirs)
                {
                    var fullPath = Path.Combine(cliente.FolderFtp, dir);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                        logger.LogDebug("Creado subdirectorio: {FullPath}", fullPath);
                    }
                }

                clientesValidos.Add(cliente);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validando carpetas para cliente {Message}", ex.Message);
        }

        return clientesValidos;
    }

    private async Task ProcesarArchivosCliente(Servidor cliente, DbContextMensajeria dbContext, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Procesando archivos en: {FolderFtp}", cliente.FolderFtp);

            var archivos = Directory.GetFiles(cliente.FolderFtp).Where(e => !Directory.Exists(e)).ToList();

            logger.LogDebug("Archivos encontrados: {Archivos}", archivos.Count);

            foreach (var archivo in archivos)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var nombreArchivo = Path.GetFileName(archivo);

                try
                {
                    logger.LogDebug("Procesando archivo: {NombreArchivo}", nombreArchivo);

                    var contenido = await File.ReadAllTextAsync(archivo, cancellationToken);

                    var hash = ExtensionesProcesamiento.GetSha256Hash(contenido);

                    logger.LogDebug("Hash calculado: {Hash}", hash);

                    var existeDuplicado = await dbContext.Mensajes.AnyAsync(e => e.Hash == hash && e.Estado == EstadoMensaje.Enviado, cancellationToken);

                    if (existeDuplicado)
                    {
                        logger.LogDebug("Archivo duplicado: {NombreArchivo}", nombreArchivo);
                        var destinoOriginal = Path.Combine(cliente.FolderFtp, "duplicados", nombreArchivo);
                        var destino = ObtenerNombreUnico(destinoOriginal);
                        File.Move(archivo, destino);
                        logger.LogDebug("Archivo movido a: {Destino}", Path.GetFileName(destino));
                    }
                    else
                    {
                        var contacto = interprete.ObtieneContacto(contenido);
                        if (contacto!.DatosValidos)
                        {
                            var mensaje = new Mensaje()
                            {
                                Id = dbContext.Mensajes.Any() ? dbContext.Mensajes.Select(e => e.Id).ToList()!.Max() + 1 : 1,
                                Hash = hash!,
                                FechaCreacion = DateTime.UtcNow,
                                Estado = EstadoMensaje.Pendiente,
                                Telefono = contacto.Telefono!,
                                NombreContacto = contacto.NombreContacto!,
                                Url = contacto.Url!,
                                ServidorId = cliente.Id,
                                SucursalId = contacto.SucursalId!
                            };

                            dbContext.Mensajes.Add(mensaje);
                            await dbContext.SaveChangesAsync();

                            string nuevoNombre = $"mensaje-{cliente.Id}-{contacto.SucursalId}-{mensaje.Id}.hl7";
                            string rutaDestino = Path.Combine(cliente.FolderFtp, "ok", nuevoNombre);

                            File.Move(archivo, rutaDestino);
                        }
                        else
                        {
                            logger.LogDebug("Archivo erroneo: {NombreArchivo}", nombreArchivo);
                            var destinoOriginal = Path.Combine(cliente.FolderFtp, "erroneos", nombreArchivo);
                            var destino = ObtenerNombreUnico(destinoOriginal);
                            File.Move(archivo, destino);
                            logger.LogDebug("Archivo movido a: {Destino}", Path.GetFileName(destino));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error procesando cliente {Message}", ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Error procesando cliente {Message}", ex.Message);
        }
    }

    private string ObtenerNombreUnico(string rutaBase)
    {
        if (!File.Exists(rutaBase)) return rutaBase;

        var directorio = Path.GetDirectoryName(rutaBase);
        var nombre = Path.GetFileNameWithoutExtension(rutaBase);

        var extension = Path.GetExtension(rutaBase);

        int contador = 1;
        string nuevaRuta;
        do
        {
            nuevaRuta = Path.Combine(directorio, $"{nombre}_{contador}{extension}");
            contador++;
        }
        while (File.Exists(nuevaRuta));

        return nuevaRuta;
    }
}