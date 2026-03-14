using comunes.auth;
using mensajeriamedica.model.identity.registro;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace mensajeriamedica.api.identity;

#pragma warning disable CA2016 // Reenviar el parámetro "CancellationToken" a los métodos
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
public class Worker(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await manager.FindByClientIdAsync("mensajeriamedica-interservicio") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "mensajeriamedica-interservicio",
                ClientSecret = "s3cr3t0",
                DisplayName = "Acceso interservicio",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials
                }
            });
        }

        if (await manager.FindByClientIdAsync("mensajeriamedica-password") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "mensajeriamedica-password",
                DisplayName = "Acceso usuarios mensajeria médica",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken
                }
            });
        }

        if (await userManager.FindByNameAsync("administrador") == null)
        {
            var admin = await userManager.CreateAsync(new ApplicationUser
            {
                UserName = "administrador",
                Email = "administrador@mensajeria.com",
                Estado = EstadoCuenta.Activo,
                Nombre = "Administrador",
                EmailConfirmed = true,
                FechaRegistro = DateTime.UtcNow,
            }, "Pa$$w0rd");

            await userManager.AddClaimAsync(await userManager.FindByNameAsync("administrador"), new System.Security.Claims.Claim("rol", "admin"));

        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
#pragma warning restore CA2016 // Reenviar el parámetro "CancellationToken" a los métodos
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
