using Azure.Monitor.OpenTelemetry.AspNetCore;
using comunes.autenticacion;
using comunes.autenticacion.abstraccion;
using comunes.proxies.proxygenerico;
using comunes.servicios.consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation.AspNetCore;
using Serilog;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace comunes.extensiones;

public static class ExtensionesConfiguracionInterservicio
{
    /// <summary>
    /// Nombre de la configuracion CORS para any origin any method any header
    /// </summary>
    public const string CORS_ANY_ORIGIN_ALL = "anyall";

    /// <summary>
    /// Obtiene una caonfiguracion de OpenId para JWT.
    /// </summary>
    /// <param name="configuracion">Configuracion de la API y clave de busqueda.</param>
    /// <param name="clave">Clave de busqueda.</param>
    /// <returns>Datos de autenticación o  nulo.</returns>
    public static AutenticacionJWT? ObtieneConfiguracionJWT(this ConfiguracionAPI configuracion, string clave)
    {
        return configuracion.AuthConfigJWT
            .FirstOrDefault(_ => _.Clave.Equals(clave, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Obtiene los datos de configuracion de un host de interservicio.
    /// </summary>
    /// <param name="configuracion">Configuracion de la API y clave de busqueda.</param>
    /// <param name="clave">Clave de busqueda.</param>
    /// <returns>Host interservicio o nulo.</returns>
    public static HostInterServicio? ObtieneHost(this ConfiguracionAPI configuracion, string clave)
    {
        return configuracion.Hosts
            .FirstOrDefault(_ => _.Clave.Equals(clave, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Inyecta el servicio de Identidad de OpenIdDict.
    /// </summary>
    /// <param name="services">Servicios del builder.</param>
    /// <param name="configuration">Configuracion para ConfiguracionAPI utiliza clave ConfiguracionAPI.</param>
    public static void InyectaOpenIdDict(this IServiceCollection services, ConfigurationManager configuration)
    {

        ConfiguracionConsul configuracionConsul = new();
        var jwtAuthorities = configuration.GetSection("JwtAuthorities").Get<List<JwtAuthorityConfiguration>>();
        List<string> issuers = jwtAuthorities?.Select(a => a.Authority.TrimEnd('/')).ToList() ?? [];
        configuration.GetSection("ConfiguracionConsul").Bind(configuracionConsul);

        if (!string.IsNullOrEmpty(configuracionConsul.CertificadoCifradoIdentity) && File.Exists(configuracionConsul.CertificadoCifradoIdentity))
        {
            X509Certificate2 encryptCert = new(configuracionConsul.CertificadoCifradoIdentity);
            foreach (var issuer in issuers)
            {
                services.AddOpenIddict()
                    .AddValidation(options =>
                    {
                        options.AddEncryptionCertificate(encryptCert);
                        options.SetIssuer(issuer);
                        options.UseSystemNetHttp();
                        options.UseAspNetCore();
                    });
            }
        }
        else
        {
            foreach (var issuer in issuers)
            {
                services.AddOpenIddict()
                .AddValidation(options =>
                {
                    options.SetIssuer(issuer);
                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });
            }
        }

        services.AddAuthorizationBuilder()
            .AddPolicy("InternalScope", policy =>
            {
                policy.AuthenticationSchemes = new[] { "Bearer" };
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "internal");
            });


        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;

        });
    }

    /// <summary>
    /// INicializa la configuracion estándar de Serilog.
    /// </summary>
    /// <param name="builder">Builder de la app.</param>
    /// <param name="configuration">Configuración de la app.</param>
    public static void InicializaSerilog(this WebApplicationBuilder builder, ConfigurationManager configuration)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddSerilog(logger);
    }


    /// <summary>
    /// DEfine una configuracion por defecto que permito el acceso sin erstricciones.
    /// </summary>
    /// <param name="builder">Builder de la app.</param>
    public static void CORSAnyAll(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                CORS_ANY_ORIGIN_ALL,
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });
    }

    /// <summary>
    /// Adiciona la configuracion estandar
    ///     Configuracion desde appsetting por entorno
    ///     Configurción de logs desde seccion Logging
    ///     Inyeccion IOptions de ConfiguracionAPI
    ///     CORS ANY ALL
    ///     Serilog v[ia configuracion
    ///     Validacion standar JWT OpenIdDict
    ///     CAche en memoria
    ///     Servicio de autenticacioón interproceso JWT
    ///     Servicio HTTPClient
    ///     AddControllers
    ///     AddEndpointsApiExplorer
    ///     AddSwaggerGen.
    /// </summary>
    /// <param name="builder">Builder de la app.</param>
    /// <param name="rootAssembly">Ensablado raíz.</param>
    public static void CreaConfiguracionStandar(this WebApplicationBuilder builder, Assembly rootAssembly)
    {
        IWebHostEnvironment environment = builder.Environment;

#if DEBUG
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                            .AddUserSecrets(rootAssembly, true)
                            .AddEnvironmentVariables();
#else
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"configuration/appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"configuration/appsettings-common.json", optional: true, reloadOnChange: true)
                            .AddUserSecrets(rootAssembly, true)
                            .AddEnvironmentVariables();
#endif

        builder.Services.AddLogging(logbuilder =>
        {
            logbuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            logbuilder.AddConsole();
        });

        if (!string.IsNullOrEmpty(builder.Configuration.GetValue<string>("AzureMonitor:ConnectionString"))
            && builder.Configuration.GetValue<bool>("AzureMonitor:Enabled"))
        {
            builder.Services.AddOpenTelemetry().UseAzureMonitor();
        }

        builder.Services.Configure<SecretosConfiguracionAPI>(builder.Configuration.GetSection("SecretosConfiguracionAPI"));
        builder.Services.Configure<ConfiguracionConsul>(builder.Configuration.GetSection("ConfiguracionConsul"));

        builder.Services.AddHealthChecks();
        builder.Services.Configure<ConfiguracionAPI>(builder.Configuration.GetSection(ConfiguracionAPI.ClaveConfiguracionBase));
        builder.CORSAnyAll();
        builder.InicializaSerilog(builder.Configuration);
        builder.Services.InyectaOpenIdDict(builder.Configuration);
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddTransient<IServicioAutenticacionJWT, ServicioAuthInterprocesoJWT>();
        builder.Services.AddTransient<IServicioConsul, ServicioConsul>();
        builder.Services.AddTransient<IProxyGenericoInterservicio, ProxyGenericoInterservicio>();
        builder.Services.AddHttpClient();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }
}