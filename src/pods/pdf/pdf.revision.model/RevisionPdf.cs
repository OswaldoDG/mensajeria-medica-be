namespace pdf.revision.model;

public class RevisionPdf
{
    /// <summary>
    /// Identificador unico de la revision del PDF.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha de inicio de la revision del archivo.
    /// </summary>
    public DateTime FechaInicioRevision { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Id del revisor.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Archivo PDF al que pertenece la parte documental.
    /// </summary>
    public int ArchivoPdfId { get; set; }

    /// <summary>
    /// Fecha de finalizacion de la revision del archivo.
    /// </summary>
    public DateTime? FechaFinRevision { get; set; }

    /// <summary>
    /// Navegacion Archivo.
    /// </summary>
    public ArchivoPdf Archivo { get; set; }
}
