using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace contabee.services.identity.configuracion;
public class ConfigTokenLoginLess : IEntityTypeConfiguration<TokenLoginLess>
{
    public void Configure(EntityTypeBuilder<TokenLoginLess> builder)
    {
        builder.ToTable(DbContextIdentity.TABLETOKENLOGINLESS);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Token).IsRequired().HasMaxLength(256);
        builder.Property(t => t.UsuarioId).IsRequired();
        builder.Property(t => t.FechaCreacion).IsRequired();
        builder.Property(t => t.UsuarioCreador).IsRequired(true);

        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(t => t.Token);
    }
}
