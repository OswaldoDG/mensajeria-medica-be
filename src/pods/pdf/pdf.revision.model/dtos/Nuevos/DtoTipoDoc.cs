using System.ComponentModel.DataAnnotations;

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

    /// <summary>
    /// Tecla asociada al documento en la UI.
    /// </summary>
    public string? Tecla { get; set; }
}
