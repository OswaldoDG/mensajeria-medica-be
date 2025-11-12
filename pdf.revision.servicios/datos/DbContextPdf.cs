using Microsoft.EntityFrameworkCore;
using pdf.revision.model;
using pdf.revision.model.dtos;

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

    public DbSet<DtoEstadistica> Estadisticas { get; set; }
    public DbContextPdf()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Ignore<DtoEstadistica>();

        modelBuilder.Entity<DtoEstadistica>().HasNoKey().ToView(null);


        // Definir la configuracion por tipo de entidad y crear la migracion.
        modelBuilder.Entity<ArchivoPdf>(entity =>
        {
            entity.ToTable("archivo_pdf");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired();
            entity.Property(e => e.Ruta).IsRequired();
            entity.Property(e => e.Estado);
            entity.Property(e => e.UltimaRevision);
            entity.Property(e => e.TotalPaginas);
            entity.Property(e => e.Prioridad);

            entity.HasMany(e => e.Partes)
                  .WithOne(p => p.Archivo)
                  .HasForeignKey(p => p.ArchivoPdfId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Revisiones)
                  .WithOne(r => r.Archivo)
                  .HasForeignKey(r => r.ArchivoPdfId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ParteDocumental>(entity =>
        {
            entity.ToTable("parte_documental");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.ArchivoPdfId);
            entity.Property(e => e.PaginaInicio);
            entity.Property(e => e.PaginaFin);
            entity.Property(e => e.TipoDocumentoId);

            entity.HasOne(e => e.TipoDocumento)
                  .WithMany(t => t.Partes)
                  .HasForeignKey(e => e.TipoDocumentoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RevisionPdf>(entity =>
        {
            entity.ToTable("revision_pdf");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.FechaInicioRevision);
            entity.Property(e => e.FechaFinRevision);
            entity.Property(e => e.UsuarioId);
            entity.Property(e => e.ArchivoPdfId);
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.ToTable("tipo_documento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired();
        });
    }
}
