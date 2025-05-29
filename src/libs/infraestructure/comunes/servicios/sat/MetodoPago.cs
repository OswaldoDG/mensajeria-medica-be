using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm.sat;

/// <summary>
/// Catalogo de MetodoPago de pago del SAT.
/// </summary>
[ExcludeFromCodeCoverage]
public class MetodoPago
{
    required public string Clave { get; set; }
    required public string Nombre { get; set; }
}
