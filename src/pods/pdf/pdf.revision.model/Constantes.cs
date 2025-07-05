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
    Cancelada = 3
}