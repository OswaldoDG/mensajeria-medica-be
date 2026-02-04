using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using mensajeriamedica.api.comunicaciones.extensiones;
using mensajeriamedica.model.comunicaciones.mensajes;
using mensajeriamedica.model.comunicaciones.servidores;
using mensajeriamedica.model.comunicaciones.whatsapp;
using mensajeriamedica.services.comunicaciones.interpretes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace mensajeriamedica.services.comunicaciones.servicios;

/// <summary>
/// Servicio primario para la creacion de mensajes.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProcesadorArchivos(ILogger<ProcesadorArchivos> logger, IInterpreteHL7 interprete, 
    IServiceScopeFactory scopeFactory, IConfiguration configuration, IDistributedCache cache,
    IOptions<WhatsAppConfig> options ) : BackgroundService
{
    private readonly int intervaloPoll = configuration.GetValue<int>("IntervaloSegundosPollFtp") * 1000;
    private const string CacheKey = "ClientesMensajeria";
    private WhatsAppConfig whatsappConfig = options.Value;
    private List<string> Folders = [];

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

        try
        {
            if (cacheD != null)
            {
                logger.LogDebug("Recuperando clientes desde caché");
                return JsonSerializer.Deserialize<List<Servidor>>(cacheD);
            }

            logger.LogInformation("Consultando Clientes desde base de datos");
            List<Servidor> clientesValidos = await dbContext.Servidores.ToListAsync(cancellationToken);

            foreach (var cliente in clientesValidos)
            {
                logger.LogDebug("Validando carpeta para cliente {Id}: {FolderFtp}", cliente.Id, cliente.FolderFtp);

                if (!Directory.Exists(cliente.FolderFtp))
                {
                    Directory.CreateDirectory(cliente.FolderFtp);
                }
            }

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            await cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(clientesValidos), options, cancellationToken);
            return clientesValidos;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validando carpetas para cliente {Message}", ex.Message);
            return [];
        }
    }

    private async Task ProcesarArchivosCliente(Servidor cliente, DbContextMensajeria dbContext, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Procesando archivos en: {FolderFtp}", cliente.FolderFtp);

            var archivos = Directory.GetFiles(cliente.FolderFtp).ToList();

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

                    var existeDuplicado = await dbContext.Mensajes.AnyAsync(e => e.Hash == hash);

                    if (existeDuplicado)
                    {
                        logger.LogDebug("Archivo duplicado: {NombreArchivo}", nombreArchivo);
                        MoveFile(archivo, cliente.FolderFtp, EstadoMensaje.Duplicado);
                    }
                    else
                    {
                        var contacto = interprete.ObtieneContacto(contenido);
                        if (contacto!.DatosValidos)
                        {

                            var mensaje = new Mensaje()
                            {
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


                            if (!string.IsNullOrEmpty(contacto.Telefono))
                            {
                                if (!mensaje.Telefono.StartsWith(whatsappConfig.PrefijoDefaultPais))
                                {
                                    mensaje.Telefono = whatsappConfig.PrefijoDefaultPais + mensaje.Telefono;
                                }

                                var wresult = await EnviaMensajeWhats(mensaje);

                                mensaje.Estado = wresult ? EstadoMensaje.Enviado : EstadoMensaje.FallidoWhatsApp;
                            }
                            else
                            {
                                mensaje.Estado = EstadoMensaje.NumTelInvalido;
                            }

                            await dbContext.SaveChangesAsync();
                            MoveFile(archivo, cliente.FolderFtp, mensaje.Estado, mensaje.Id);

                        }
                        else
                        {
                            logger.LogDebug("Archivo erroneo: {NombreArchivo}", nombreArchivo);
                            MoveFile(archivo, cliente.FolderFtp, EstadoMensaje.Fallido);
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

    private void MoveFile(string sourcePath, string rutaBase,  EstadoMensaje estado, long? dbId = null)
    {

        string destino = string.Empty;
        string subFolder = Path.Combine(rutaBase, $"{estado}");

        if (this.Folders.IndexOf(subFolder) < 0)
        {
            if (!Directory.Exists(subFolder))
            {
                Directory.CreateDirectory(subFolder);
            }
            this.Folders.Add(subFolder);
        }

        try
        {
            var nombreArchivo = Path.GetFileName(sourcePath);
            switch (estado)
            {

                case EstadoMensaje.FallidoWhatsApp:
                case EstadoMensaje.Enviado:
                    nombreArchivo = $"msg-{dbId}.hl7";
                    destino = Path.Combine(subFolder, nombreArchivo);
                    break;

                case EstadoMensaje.Duplicado:
                case EstadoMensaje.Fallido:
                    destino = Path.Combine(subFolder, nombreArchivo);
                    break;
            }

            if (destino != string.Empty)
            {
                if (File.Exists(destino))
                {
                    destino = destino + "." + DateTime.UtcNow.Ticks;
                }
                File.Move(sourcePath, destino);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando archivo {Archivo} -> {Destino} {Message}", sourcePath, destino, ex.Message);
        }
    }

    private async Task<bool> EnviaMensajeWhats(Mensaje m)
    {
        var accessToken = this.whatsappConfig.AccessToken;
        var phoneNumberId = this.whatsappConfig.PhoneNumber;
        var recipient = m.Telefono; 

        using var client = new HttpClient();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://graph.facebook.com/v22.0/{phoneNumberId}/messages"),
            Headers = { { "Authorization", $"Bearer {accessToken}" } },
            Content = JsonContent.Create(new
            {
                messaging_product = "whatsapp",
                to = recipient,
                type = "template",
                template = new
                {
                    name = this.whatsappConfig.TemplateId, // replace with your approved template name
                    language = new { code = "es_MX" },
                    components = new[]
                    {
                        new {
                            type = "header",
                            parameters = new[]
                            {
                                new { type = "text", text = "Hospítal Radiológico" }, // dynamic parameter
                            }
                        },
                        new {
                            type = "body",
                            parameters = new[]
                            {
                                new { type = "text", text = m.NombreContacto }, // dynamic parameter
                                new { type = "text", text = m.Url }
                            }
                        }
                    }
                }
            })
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            logger.LogError("Error procesando whatsapp {Message}", result);
        }

        return response.IsSuccessStatusCode;
    }
}