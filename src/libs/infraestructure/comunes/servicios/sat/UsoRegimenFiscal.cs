using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm.sat;

/// <summary>
/// Usos del cfdi pr regimen fiscal.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsoRegimenFiscal
{
    required public string ClaveRegimen { get; set; }

    public List<UsoCfdi> Usos { get; set; } = [];
}
