using System.Diagnostics.CodeAnalysis;

namespace comunes.busqueda;

/// <summary>
/// DTO de resultados de búsqueda que contiene un identificador y un texto asociado.
/// </summary>
[ExcludeFromCodeCoverage]
public class ParIdTexto
{
    required public string Id { get; set; }
    required public string Texto { get; set; }
}
