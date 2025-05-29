namespace contabee.model.identity.usuarios;

public class DTORecuperaContrasena
{
    /// <summary>
    /// Email con el que el usuario fue dado de alta.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Email del usuario.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Token de recuperaciòn generado.
    /// </summary>
    public string? TokenRecuperacion { get; set; }
}
