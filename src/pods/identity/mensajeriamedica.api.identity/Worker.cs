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
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        List<IdentityRole> requiredRoles =
        [
            new IdentityRole(ConstantesSeguridad.RolAdminSistema),
        ];

        var existingRoles = await roleManager.Roles.ToListAsync();

        foreach (var role in requiredRoles)
        {
            if (!existingRoles.Any(r => r.Name == role.Name))
            {
                await roleManager.CreateAsync(new IdentityRole(role.Name!));
            }
        }

        if (await manager.FindByClientIdAsync("contabee-interservicio") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "contabee-interservicio",
                ClientSecret = "s3cr3t0",
                DisplayName = "Acceso interservicio",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials
                }
            });
        }

        if (config.GetValue<bool>("MockData"))
        {
            string email = "admin@interno.com";
            ApplicationUser? user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    Id = "00000000-0000-0000-0000-000000000001",
                    UserName = email,
                    Email = email,
                    FechaActivacion = DateTime.UtcNow,
                    EmailConfirmed = true,
                    Estado = EstadoCuenta.Activo,
                    Nombre = "Administrador sistema",
                    TipoCuenta = TipoCuenta.Empleado,
                };
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, ConstantesSeguridad.RolAdminSistema);
            }
        }

        if (await manager.FindByClientIdAsync("contabee-password") == null)
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
