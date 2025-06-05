using mensajeriamedica.model.identity.usuarios;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mensajeriamedica.services.identity.configuracion;

/// <summary>
/// Configuraci[on de la entidad.
/// </summary>
public class ConfigRolCuentaFiscal : IEntityTypeConfiguration<RolCuentaFiscal>
{
    public void Configure(EntityTypeBuilder<RolCuentaFiscal> builder)
    {
        builder.ToTable(DbContextIdentity.TABLAUSUARIOSROLCUENTAFISCAL);
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.CuentaFiscalId, e.UsuarioId });
        builder.Property(e => e.UsuarioId).IsRequired(true);
        builder.Property(e => e.CuentaFiscalId).IsRequired(true);
        builder.Property(e => e.Roles).IsRequired(true).HasMaxLength(250);
        builder.HasIndex(e => new { e.CuentaFiscalId, e.UsuarioId });
    }
}
