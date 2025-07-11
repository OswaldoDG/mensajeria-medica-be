using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using comunes;
using comunes.autenticacion;
using comunes.autenticacion.abstraccion;
using comunes.proxies.proxygenerico;
using comunes.servicios.consul;
using mensajeriamedica.api.identity;
using mensajeriamedica.api.identity.helpers;
using mensajeriamedica.api.identity.models;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.services.identity.dbcontext;
using mensajeriamedica.services.identity.registro;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

public class Program
{
    private static void InyectaOpenIdDict(IServiceCollection services, ConfigurationManager configuration)
    {
        ConfiguracionServicioIdentidad configIdentidad = new ();
        configuration.GetSection("ConfiguracionServicioIdentidad").Bind(configIdentidad);
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                //options.SetIssuer(configIdentidad!.UrlToken!);
                //options.UseSystemNetHttp();
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization();
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        IWebHostEnvironment environment = builder.Environment;

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables();
        var configuration = builder.Configuration;

        ConfiguracionServicioIdentidad configIdentidad = new ();
        configuration.GetSection("configIdentidad").Bind(configIdentidad);

        bool continuar = IdentityHelpers.PreProceso(args, configIdentidad);
        if (!continuar)
        {
            Console.WriteLine("PreProceso finalizado");
            return;
        }

        // Add services to the container.
        builder.Services.AddCors(c =>
        {
            c.AddPolicy("default", p =>
            {
                p.AllowAnyMethod();
                p.AllowAnyOrigin();
                p.AllowAnyHeader();
            });
        });

        // Add services to the container.
        // builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<DbContextIdentity>(options =>
        {
            // Configure the context to use mysql.
            var connectionString = builder.Configuration.GetConnectionString("identityMySql");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            options.UseOpenIddict();
        });

        // Register the Identity services.
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<DbContextIdentity>()
            .AddDefaultTokenProviders();

        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
        // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
        builder.Services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        builder.Services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<DbContextIdentity>();

                // Enable Quartz.NET integration.
                options.UseQuartz();
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the token endpoint.
                options.SetAuthorizationEndpointUris("connect/authorize")
                     .SetLogoutEndpointUris("connect/logout")
                     .SetIntrospectionEndpointUris("connect/introspect")
                     .SetTokenEndpointUris("connect/token")
                     .SetUserinfoEndpointUris("connect/userinfo")
                     .SetVerificationEndpointUris("connect/verify");

                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, Scopes.OfflineAccess);

                // Enable the password and the refresh token flows.
                options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow()
                       .AllowClientCredentialsFlow();

                // Accept anonymous clients (i.e clients that don't send a client_id).
                // options.AcceptAnonymousClients();

                // REgistra credenciales  de  cifrado para el JWT.
                if (!string.IsNullOrEmpty(configIdentidad.EncryptionCertificate) && File.Exists(configIdentidad.EncryptionCertificate))
                {
                    Console.WriteLine($"Utilizando certificado de cifrado {configIdentidad.EncryptionCertificate}");
                    X509Certificate2 ec = new(configIdentidad.EncryptionCertificate);
                    options.AddEncryptionCertificate(ec);
                }
                else
                {
                    Console.WriteLine("Utilizando certificado de desarrollo para cifrado");
                    options.AddDevelopmentEncryptionCertificate();
                }

                // REgistra credenciales de firma para el JWT
                if (!string.IsNullOrEmpty(configIdentidad.SigningCertificate) && File.Exists(configIdentidad.SigningCertificate))
                {
                    Console.WriteLine($"Utilizando certificado para firma {configIdentidad.SigningCertificate} {configIdentidad.SigningPassword} ");
                    X509Certificate2 sc = new(configIdentidad.SigningCertificate, configIdentidad.SigningPassword);
                    options.AddSigningCertificate(sc);
                }
                else
                {
                    Console.WriteLine("Utilizando certificado de desarrollo para firma");
                    options.AddDevelopmentSigningCertificate();
                }

                // Evita que se encripte el payload del token
                if (!configIdentidad.JWTCifrado)
                {
                    options.DisableAccessTokenEncryption();
                }

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserinfoEndpointPassthrough()
                       .EnableStatusCodePagesIntegration();
            });

        InyectaOpenIdDict(builder.Services, builder.Configuration);

        // Register the worker responsible for seeding the database.
        // Note: in a real world application, this step should be part of a setup script.
        builder.Services.AddHostedService<Worker>();

        // Add httpClient for outgoing requests
        builder.Services.AddHttpClient();
        builder.Services.AddDistributedMemoryCache();

        builder.Services.Configure<SecretosConfiguracionAPI>(builder.Configuration.GetSection("SecretosConfiguracionAPI"));
        builder.Services.Configure<ConfiguracionAPI>(builder.Configuration.GetSection("ConfiguracionAPI"));
        builder.Services.Configure<ConfiguracionConsul>(builder.Configuration.GetSection("ConfiguracionConsul"));
        builder.Services.AddTransient<IServicioConsul, ServicioConsul>();
        builder.Services.AddTransient<IServicioAutenticacionJWT, ServicioAuthInterprocesoJWT>();
        builder.Services.AddTransient<IProxyGenericoInterservicio, ProxyGenericoInterservicio>();
        builder.Services.AddTransient<IServicioRegistro, ServicioRegistro>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks();

        if (!string.IsNullOrEmpty(builder.Configuration.GetValue<string>("AzureMonitor:ConnectionString"))
            && builder.Configuration.GetValue<bool>("AzureMonitor:Enabled"))
        {
            builder.Services.AddOpenTelemetry().UseAzureMonitor();
        }

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContextIdentity>();
        dbContext.Database.Migrate();

        app.UseRouting();
        app.UseCors("default");
        app.MapHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapDefaultControllerRoute();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Run();
    }
}