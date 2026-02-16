using mensajeriamedica.model.comunicaciones.centroscostos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.comunicaciones.configuracion
{
    public class ConfigUnidadCostos : IEntityTypeConfiguration<UnidadCostos>
    {
        public void Configure(EntityTypeBuilder<UnidadCostos> builder)
        {
            builder.ToTable(DbContextMensajeria.Tabla_UNIDADCOSTOS);
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).IsRequired(true);
            builder.Property(u => u.CentroCostosId).IsRequired(true);
            builder.Property(u => u.Nombre).HasMaxLength(200).IsRequired(true);
            builder.Property(u => u.Clave).HasMaxLength(50).IsRequired(true);

            builder.HasOne(u => u.CentroCostos).WithMany(c => c.Unidades).HasForeignKey(u => u.CentroCostosId);

            builder.HasIndex(i => i.Id);
        }
    }
}
