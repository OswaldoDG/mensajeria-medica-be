using mensajeriamedica.model.identity.registro;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace contabee.model.identity.tokenloginless;

/// <summary>
/// Define tokens de acceso para usuarios sin login.
/// </summary>
[ExcludeFromCodeCoverage]
public class TokenLoginLess
{
    /// <summary>
    /// Identificádor único del toke.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// IDentificador único del usuario al que se asocia el token.
    /// </summary>
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
    public string UsuarioId { get; set; }
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.

    /// <summary>
    /// Token de acceso para intercambiar por un JWT.
    /// </summary>
    required public string Token { get; set; }

    /// <summary>
    /// FEcha de creación del token.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Id del usuario creador del token.
    /// </summary>
    public Guid? UsuarioCreador { get; set; }
}
