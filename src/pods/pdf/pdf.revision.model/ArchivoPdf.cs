namespace pdf.revision.model;

/// <summary>
/// DEfine un archivo PDF a separar.
/// </summary>
public class ArchivoPdf
{
    /// <summary>
    /// IDentificador unico del archivo.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del archivo.
    /// </summary>
    required public string Nombre { get; set; }

    /// <summary>
    /// Ruta del archivo en el sistema de archivos.
    /// </summary>
    required public string Ruta { get; set; }

    /// <summary>
    /// Estado de la revision del archivo.
    /// </summary>
    public EstadoRevision Estado { get; set; } = EstadoRevision.Pendiente;

    /// <summary>
    /// Fecha de la revision mas reciente del archivo PDF.
    /// </summary>
    public DateTime? UltimaRevision { get; set; }

    /// <summary>
    /// Total de paginas del archivo PDF.
    /// </summary>
    public int TotalPaginas { get; set; } = 0;

    /// <summary>
    /// Navegacion a la lista de partes asociadas al archivo Pdf.
    /// </summary>
    public List<ParteDocumental> Partes { get; set; }

    /// <summary>
    /// Navegacion a la lista de revisiones asociadas al archivo Pdf.
    /// </summary>
    public List<RevisionPdf> Revisiones { get; set; }

    /// <summary>
    /// Prioridad de proceasamiento, enntre mas alto el numero, mayor prioridad tiene el archivo para ser procesado.
    /// </summary>
    public int Prioridad { get; set; }
}
