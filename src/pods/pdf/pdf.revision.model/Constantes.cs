namespace pdf.revision.model;

public enum EstadoRevision
{
    /// <summary>
    /// Revision pendiente.
    /// </summary>
    Pendiente = 0,

    /// <summary>
    /// Revision en curso.
    /// </summary>
    EnCurso = 1,

    /// <summary>
    /// Revision finalizada.
    /// </summary>
    Finalizada = 2,
 
    /// <summary>
    /// Revision cancelada.
    /// </summary>
    Cancelada = 3,

    /// <summary>
    /// Indica que el archivo ha sido separado en varios PDFs.
    /// </summary>
    SeparadoEnPdfs = 4,

    /// <summary>
    /// Proceso de separación.
    /// </summary>
    ProcesoPDF = 5,

    /// <summary>
    /// Documento sin partes.
    /// </summary>
    SinPartes = 6,

    /// <summary>
    /// Documento excluido.
    /// </summary>
    Excluido = 10,

    /// <summary>
    /// Documento para reproceso.
    /// </summary>
    Reproceso = 11,

    /// <summary>
    /// Sin PDF.
    /// </summary>
    SinPDF = 12,

    /// <summary>
    /// Estado finalizado sin errores.
    /// </summary>
    FinalizadaConErrores = 13
}