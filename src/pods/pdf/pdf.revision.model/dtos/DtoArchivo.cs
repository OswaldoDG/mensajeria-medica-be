namespace pdf.revision.model.dtos;

/// <summary>
/// Dto para el frontend que representa un archivo PDF.
/// </summary>
public class DtoArchivo
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
    /// TokenSAS del contenedor.
    /// </summary>
    public string TokenSAS { get; set; } = string.Empty;

}
