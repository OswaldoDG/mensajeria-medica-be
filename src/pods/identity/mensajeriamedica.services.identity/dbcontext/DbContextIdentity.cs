using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.model.identity.usuarios;
using mensajeriamedica.services.identity.configuracion;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace mensajeriamedica.services.identity.dbcontext;

public class DbContextIdentity(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public const string TABLAUSUARIOSROLCUENTAFISCAL = "seguridad$usuariosrolcf";
    public const string TABLAUSUARIO = "aspnetusers";
    public const string TABLAROLES = "aspnetuserroles";
    public const string TABLETOKENLOGINLESS = "seguridad$tokensloginless";
    public const string TABLADISPOSITIVOUSUARIO = "seguridad$dispusuario";
    public const string TABLATOKENVINCULACION = "seguridad$vinculacion";

    public DbSet<RolCuentaFiscal> RolesCuentaFiscal { get; set; }
    public DbSet<TokenLoginLess> TokensLoginless { get; set; }
    public DbSet<DispositivoUsuario> DispositivosUsuario { get; set; }
    public DbSet<TokenVinculacion> TokensVinculacion { get; set; }

#pragma warning disable S1185 // Overriding members should do more than simply call the same member in the base class
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ConfigRolCuentaFiscal());
        builder.ApplyConfiguration(new ConfigTokenLoginLess());
        builder.ApplyConfiguration(new ConfigDispositivoUsuario());
        builder.ApplyConfiguration(new ConfigTokenVinculacion());

        base.OnModelCreating(builder);
    }
#pragma warning restore S1185 // Overriding members should do more than simply call the same member in the base class
}
