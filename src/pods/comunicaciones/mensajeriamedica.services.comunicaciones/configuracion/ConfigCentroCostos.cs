using mensajeriamedica.model.comunicaciones.centroscostos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.comunicaciones.configuracion
{
    public class ConfigCentroCostos : IEntityTypeConfiguration<CentroCostos>
    {
        public void Configure(EntityTypeBuilder<CentroCostos> builder)
        {
            builder.ToTable(DbContextMensajeria.Tabla_CENTROCOSTOS);
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).IsRequired(true);
            builder.Property(c => c.Nombre).HasMaxLength(200).IsRequired(true);
            builder.Property(c => c.Eliminado).HasDefaultValue(false).IsRequired(true);

            builder.HasMany(c => c.Unidades).WithOne(u => u.CentroCostos).HasForeignKey(u => u.CentroCostosId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(c => c.Usuarios).WithOne(user => user.CentroCostos).HasForeignKey(c => c.CentroCostosId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Id);
        }

    }
}
