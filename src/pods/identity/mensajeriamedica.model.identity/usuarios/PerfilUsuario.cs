namespace mensajeriamedica.model.identity.usuarios;

/// <summary>
/// DTO del perfil de usuario para las UIs.
/// </summary>
public class PerfilUsuario
{
    /// <summary>
    /// Nombre de despliegue.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Iniciales del usuario.
    /// </summary>
    public string? Iniciales { get; set; }

    /// <summary>
    /// Especifica si el usuario es interno.
    /// </summary>
    public bool EsInterno { get; set; }

    /// <summary>
    /// Lista de roles del usuario.
    /// </summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Identificador de la cuenta fiscal.
    /// </summary>
    public Guid? CuentaFiscalId { get; set; }

}
