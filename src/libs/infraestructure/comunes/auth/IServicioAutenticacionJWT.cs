namespace comunes.autenticacion.abstraccion;

/// <summary>
/// Servico de autneticación.
/// </summary>
public interface IServicioAutenticacionJWT
{
    /// <summary>
    /// Obtiene un token interproceso a aprtir de la configuración utilizando la clave de Autenticacion JWT
    /// </summary>
    /// <param name="claveConfiguracion">Clave de configuracipon para el servicio de identidad.</param>
    /// <returns>Token interpoceso o nulo.</returns>
    Task<TokenJWT?> TokenInterproceso(string claveConfiguracion = "auth_default");
}
