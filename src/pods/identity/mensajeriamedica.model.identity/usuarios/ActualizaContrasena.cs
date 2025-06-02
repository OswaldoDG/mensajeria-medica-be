namespace mensajeriamedica.model.identity.usuarios;

/// <summary>
/// DTO para actualizar contraseña.
/// </summary>
public class ActualizaContrasena
{
    /// <summary>
    /// Correo del usuario.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Contraseña nueva.
    /// </summary>
    public required string NuevaContrasena { get; set; }

    /// <summary>
    /// Token de recuperacion.
    /// </summary>
    public required string TokenRecuperacion { get; set; }
}
