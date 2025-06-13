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
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
#pragma warning restore CA2016 // Reenviar el parámetro "CancellationToken" a los métodos
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
