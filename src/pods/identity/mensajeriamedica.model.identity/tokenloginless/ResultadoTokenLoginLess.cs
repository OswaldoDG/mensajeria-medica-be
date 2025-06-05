using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.identity.tokenloginless;

/// <summary>
/// DTO para la transferencia del token.
/// </summary>
[ExcludeFromCodeCoverage]
public class ResultadoTokenLoginLess
{
    /// <summary>
    /// IDentificador único del usuario al que se asocia el token.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Token de acceso para intercambiar por un JWT.
    /// </summary>
    required public string Token { get; set; }
}
