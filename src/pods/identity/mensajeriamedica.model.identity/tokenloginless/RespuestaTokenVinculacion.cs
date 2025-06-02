using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.identity.tokenloginless;

/// <summary>
/// DTO de respuesta par aun token loginless.
/// </summary>
[ExcludeFromCodeCoverage]
public class RespuestaTokenVinculacion
{
    required public string Token { get; set; }
    public DateTime Fecha { get; set; }
}
