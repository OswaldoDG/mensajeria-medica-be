using mensajeriamedica.model.identity.registro;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace mensajeriamedica.services.identity.dbcontext;

public class DbContextIdentity(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{

#pragma warning disable S1185 // Overriding members should do more than simply call the same member in the base class
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
#pragma warning restore S1185 // Overriding members should do more than simply call the same member in the base class
}
