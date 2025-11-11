using System.Text.Json.Serialization;

namespace pdf.revision.model;

/// <summary>
/// Representa una parte documental asociada a un PDF.
/// </summary>
public class ParteDocumental
{
    /// <summary>
    /// Identificador unico de la parte documental.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Archivo PDF al que pertenece la parte documental.
    /// </summary>
    public int ArchivoPdfId { get; set; }

    /// <summary>
    /// Numero de pagina en donde comienza la parte deocumental.
    /// </summary>
    public int PaginaInicio { get; set; }


    /// <summary>
    /// Numero de pagina en donde finaliza la parte deocumental.
    /// </summary>
    public int PaginaFin { get; set; }

    /// <summary>
    /// Identificador unico de tipo de documento.
    /// </summary>
    public int TipoDocumentoId { get; set; }

    /// <summary>
    /// Especifica si el elemento pertenee a un grupo.
    /// </summary>
    public int? IdAgrupamiento { get; set; }

    /// <summary>
    /// Navegacion Archivo.
    /// </summary>
    public ArchivoPdf Archivo { get; set; }

    /// <summary>
    /// Navegacion tipo documento.
    /// </summary>
    public TipoDocumento TipoDocumento { get; set; }
}
