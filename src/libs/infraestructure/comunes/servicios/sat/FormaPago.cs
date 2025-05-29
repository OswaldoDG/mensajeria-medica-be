using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm.sat;

/// <summary>
/// Catalogo de formas de pago del SAT.
/// </summary>
[ExcludeFromCodeCoverage]
public class FormaPago
{
    required public string Clave { get; set; }
    required public string Nombre { get; set; }
}
