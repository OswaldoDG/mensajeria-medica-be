using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm.sat;

/// <summary>
/// CAtálogo uso de CFDI del SAT.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsoCfdi
{
    required public string Clave { get; set; }
    required public string Nombre { get; set; }
    public bool AplicaPFisica { get; set; }
    public bool AplicaPMoral { get; set; }
    public string InicioVigencia { get; set; } = "01/01/2022";
    public string? FinVigencia { get; set; }
    public string RegimenReceptor { get; set; } = "";
}