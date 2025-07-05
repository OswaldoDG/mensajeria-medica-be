namespace pdf.revision.model;

/// <summary>
/// Tipos de documento que pueden ser revisados.
/// </summary>
public class TipoDocumento
{
    /// <summary>
    /// Identificador unico del tipo de documento.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del tipo de documento.
    /// </summary>
    required public string Nombre { get; set; }

    /// <summary>
    /// Navegacion a la lista de partes que son del tipo documento.
    /// </summary>
    public List<ParteDocumental> Partes { get; set; }
}
