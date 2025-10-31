namespace pdf.revision.model.dtos;

/// <summary>
/// Dto de finalizacion de separacion.
/// </summary>
public class DtoFinalizar
{
    /// <summary>
    /// Lista de partes documentales que componen el PDF.
    /// </summary>
    public List<DtoParteDocumental> Partes { get; set; } = [];

    /// <summary>
    /// Total de paginas del PDF que se esta separando.
    /// </summary>
    public int TotalPaginas { get; set; }
}
