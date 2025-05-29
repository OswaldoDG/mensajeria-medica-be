using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.usuarios;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace contabee.services.identity.configuracion;
public class ConfigDispositivoUsuario : IEntityTypeConfiguration<DispositivoUsuario>
{
    public void Configure(EntityTypeBuilder<DispositivoUsuario> builder)
    {
        builder.ToTable(DbContextIdentity.TABLADISPOSITIVOUSUARIO);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.UsuarioId).IsRequired().HasMaxLength(256);
        builder.Property(t => t.DispositivoId).IsRequired().HasMaxLength(256); ;
        builder.Property(t => t.FechaAsociacion).IsRequired();

        builder.HasOne<ApplicationUser>(x => x.Usuario).WithMany(y => y.DispositivosUsuario).HasForeignKey(z => z.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(t => t.UsuarioId);
    }
}
