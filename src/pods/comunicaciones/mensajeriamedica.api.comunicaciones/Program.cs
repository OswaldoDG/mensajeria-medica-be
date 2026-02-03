using System.Reflection;
using System.Text.Json.Serialization;
using comunes.extensiones;
using mensajeriamedica.model.comunicaciones.whatsapp;
using mensajeriamedica.services.comunicaciones;
using mensajeriamedica.services.comunicaciones.interpretes;
using mensajeriamedica.services.comunicaciones.servicios;
using Microsoft.EntityFrameworkCore;

#pragma warning disable S1118 // Utility classes should not have public constructors
#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable CA1050 // Declarar tipos en espacios de nombres
public class Program
#pragma warning restore CA1050 // Declarar tipos en espacios de nombres
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore S1118 // Utility classes should not have public constructors
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.CreaConfiguracionStandar(Assembly.GetExecutingAssembly());
        var connectionString = builder.Configuration.GetConnectionString("mensajeria-medica");

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddDbContext<DbContextMensajeria>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<ProcesadorArchivos>();
        builder.Services.AddTransient<IServicioMensajes, ServicioMensajes>();
        builder.Services.AddSingleton<IInterpreteHL7, InterpreteHL7>();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.Configure<WhatsAppConfig>(builder.Configuration.GetSection("ConfiguracionWhatsApp"));

        var app = builder.Build();
        Seed(app);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapHealthChecks("/health");
        app.UseCors(ExtensionesConfiguracionInterservicio.CORS_ANY_ORIGIN_ALL);
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    public static void Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContextMensajeria>();
        dbContext.Database.Migrate();
     }
}
