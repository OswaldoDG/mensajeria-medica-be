using mensajeriamedica.model.comunicaciones.mensajes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.comunicaciones.configuracion;

public class ConfigEstadisticasMensajes : IEntityTypeConfiguration<EstadisticasMensajes>
{
    public void Configure(EntityTypeBuilder<EstadisticasMensajes> builder)
    {
        builder.ToTable(DbContextMensajeria.Tabla_ESTADISICASMENSAJES);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired(true);
        builder.Property(e => e.ServidorId).IsRequired(true);
        builder.Property(e => e.SucursalId).IsRequired(false).HasMaxLength(100);
        builder.Property(e => e.Ano).IsRequired(true);
        builder.Property(e => e.Mes).IsRequired(true);
        builder.Property(e => e.Dia).IsRequired(true);
        builder.Property(e => e.Procesados).IsRequired(true);
        builder.Property(e => e.Enviados).IsRequired(true);
        builder.Property(e => e.Erroneos).IsRequired(true);
        builder.Property(e => e.Duplicados).IsRequired(true);

        builder.HasIndex(i => i.Id);
    }
}
