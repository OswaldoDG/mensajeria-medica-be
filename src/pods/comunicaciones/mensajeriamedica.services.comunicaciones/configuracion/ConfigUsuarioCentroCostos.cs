using mensajeriamedica.model.comunicaciones.centroscostos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.comunicaciones.configuracion
{
    public class ConfigUsuarioCentroCostos : IEntityTypeConfiguration<UsuarioCentroCostos>
    {
        public void Configure(EntityTypeBuilder<UsuarioCentroCostos> builder)
        {
            builder.ToTable(DbContextMensajeria.Tabla_USUARIOCENTROCOSTOS);
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Id).IsRequired(true);
            builder.Property(user => user.CentroCostosId).IsRequired(true);
            builder.Property(user => user.UsuarioId).IsRequired(true);

            builder.HasOne(user => user.CentroCostos).WithMany(c => c.Usuarios).HasForeignKey(user => user.CentroCostosId);

            builder.HasIndex(i => i.Id);
        }
    }
}
