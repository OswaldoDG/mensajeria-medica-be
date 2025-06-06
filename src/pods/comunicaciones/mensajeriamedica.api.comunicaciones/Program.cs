using System.Reflection;
using comunes.extensiones;
using mensajeriamedica.services.comunicaciones.interpretes;
using mensajeriamedica.services.comunicaciones.servicios;

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
        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<ProcesadorArchivos>();
        builder.Services.AddSingleton<IInterpreteHL7, InterpreteHL7>();
        builder.Services.AddDistributedMemoryCache();

        var app = builder.Build();

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
}
