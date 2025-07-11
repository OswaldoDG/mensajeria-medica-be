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
        modelBuilder.Entity<ArchivoPdf>(entity =>
        {
            entity.ToTable("archivo_pdf");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired();
            entity.Property(e => e.Ruta).HasColumnName("ruta").IsRequired();
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.UltimaRevision).HasColumnName("ultima_revision");
            entity.Property(e => e.TotalPaginas).HasColumnName("total_paginas");
            entity.Property(e => e.Prioridad).HasColumnName("prioridad");

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
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArchivoPdfId).HasColumnName("archivo_pdf_id");
            entity.Property(e => e.PaginaInicio).HasColumnName("pagina_inicio");
            entity.Property(e => e.PaginaFin).HasColumnName("pagina_fin");
            entity.Property(e => e.TipoDocumentoId).HasColumnName("tipo_documento_id");

            entity.HasOne(e => e.TipoDocumento)
                  .WithMany(t => t.Partes)
                  .HasForeignKey(e => e.TipoDocumentoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RevisionPdf>(entity =>
        {
            entity.ToTable("revision_pdf");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaInicioRevision).HasColumnName("fecha_inicio_revision");
            entity.Property(e => e.FechaFinRevision).HasColumnName("fecha_fin_revision");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.ArchivoPdfId).HasColumnName("archivo_pdf_id");
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.ToTable("tipo_documento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired();
        });
    }
}
