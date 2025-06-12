using mensajeriamedica.model.comunicaciones.mensajes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.comunicaciones.configuracion;

public class ConfigMensaje : IEntityTypeConfiguration<Mensaje>
{
    public void Configure(EntityTypeBuilder<Mensaje> builder)
    {
        builder.ToTable(DbContextMensajeria.TABLA_MENSAJES);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired(true);
        builder.Property(e => e.Hash).IsRequired(true).HasMaxLength(128);
        builder.Property(e => e.FechaCreacion).IsRequired(true);
        builder.Property(e => e.Estado).IsRequired(true);
        builder.Property(e => e.Telefono).IsRequired(true).HasMaxLength(15);
        builder.Property(e => e.NombreContacto).IsRequired(true).HasMaxLength(100);
        builder.Property(e => e.Url).IsRequired(true).HasMaxLength(300);
        builder.Property(e => e.ServidorId).IsRequired(true);
        builder.Property(e => e.SucursalId).IsRequired(true).HasMaxLength(100);

        builder.HasIndex(i => i.Hash);
        builder.HasIndex(i => i.FechaCreacion);
        builder.HasIndex(i => i.Estado);
        builder.HasIndex(i => i.SucursalId);
        builder.HasIndex(i => new { i.Hash, i.FechaCreacion, i.Estado, i.SucursalId });

        builder.HasOne(x => x.Servidor).WithMany(y => y.Mensajes).HasForeignKey(z => z.ServidorId).OnDelete(DeleteBehavior.Restrict);
    }
}
