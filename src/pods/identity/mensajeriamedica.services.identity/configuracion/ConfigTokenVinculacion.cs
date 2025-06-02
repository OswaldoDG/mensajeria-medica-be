using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.identity.configuracion;

public class ConfigTokenVinculacion : IEntityTypeConfiguration<TokenVinculacion>
{
    public void Configure(EntityTypeBuilder<TokenVinculacion> builder)
    {
        builder.ToTable(DbContextIdentity.TABLATOKENVINCULACION);
        builder.HasKey(e => e.DeviceId);
        builder.Property(e => e.DeviceId).HasMaxLength(256).IsRequired(true);
        builder.Property(e => e.Token).HasMaxLength(10).IsRequired(true);
        builder.Property(e => e.Fecha).IsRequired(true);
        builder.Property(e => e.Activado).IsRequired(true);
        builder.HasIndex(e => new { e.Token, e.Fecha });
    }
}
