using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.services.comunicaciones.interpretes;
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
    private readonly int intervaloPoll = configuration.GetValue<int>("IntervaloPollProcesadorArchivos") * 1000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContextMensajeria>();

                logger.LogDebug("Iniciando procesamiento de archivos...");
                await Task.Delay(intervaloPoll, stoppingToken);
            }
        }
    }
}
