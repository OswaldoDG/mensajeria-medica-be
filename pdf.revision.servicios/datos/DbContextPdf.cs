using Microsoft.EntityFrameworkCore;
using pdf.revision.model;

namespace pdf.revision.servicios.datos;

public class DbContextPdf : DbContext
{
    public DbContextPdf(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ArchivoPdf> Archivos { get; set; }
    public DbSet<TipoDocumento> TiposDocumento { get; set; }
    public DbSet<ParteDocumental> PartesArchivo { get; set; }
    public DbSet<RevisionPdf> Revisiones { get; set; }

    public DbContextPdf()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Definir la configuracion por tipo de entidad y crear la migracion.
    }
}
