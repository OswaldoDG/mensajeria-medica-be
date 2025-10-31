namespace pdf.revision.model.dtos.Nuevos;

public class DtoTipoDoc
{
    /// <summary>
    /// Identificador unico del tipo de documento.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del tipo de documento.
    /// </summary>
    required public string Nombre { get; set; }
}
