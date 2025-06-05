namespace mensajeriamedica.model.identity.administracion;

public class ActualizaContrasenaEmpleado
{
    /// <summary>
    /// Identificado único del empleado.
    /// </summary>
    public Guid EmpleadoId { get; set; }

    /// <summary>
    /// Nueva contraseña.
    /// </summary>
    required public string NuevoPassword { get; set; }
}
