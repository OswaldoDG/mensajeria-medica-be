using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm.sat;

/// <summary>
/// Catalogo de RegimenFiscal de pago del SAT.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegimenFiscal
{
    required public string Clave { get; set; }
    required public string Nombre { get; set; }
    public bool PMoral { get; set; }
    public bool PFisica { get; set; }
    public string InicioVigencia { get; set; } = "01/01/2022";
    public string? FinVigencia { get; set; }
}
