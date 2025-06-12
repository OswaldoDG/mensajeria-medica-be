using mensajeriamedica.model.comunicaciones.servidores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.comunicaciones.configuracion;

public class ConfigServidor : IEntityTypeConfiguration<Servidor>
{
    public void Configure(EntityTypeBuilder<Servidor> builder)
    {
        builder.ToTable(DbContextMensajeria.TABLA_SERVIDORES);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired(true);
        builder.Property(e => e.Nombre).IsRequired(true).HasMaxLength(100);
        builder.Property(e => e.FolderFtp).IsRequired(true).HasMaxLength(250);
    }
}
