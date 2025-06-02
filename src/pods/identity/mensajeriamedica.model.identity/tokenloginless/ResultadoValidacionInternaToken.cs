using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.identity.tokenloginless;

/// <summary>
/// DTO para la validacion interna del token.
/// </summary>
[ExcludeFromCodeCoverage]
public class ResultadoValidacionInternaToken
{
    /// <summary>
    /// Validez del token.
    /// </summary>
    public bool Valido { get; set; }

    /// <summary>
    /// Usuario asociado al token.
    /// </summary>
    public Guid? UsuarioId { get; set; }
}
