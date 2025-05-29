namespace contabee.model.identity.registro;

[Flags]
public enum TipoCuenta
{
    /// <summary>
    /// El suario no tiene tipo asignado.
    /// </summary>
    None = 0,

    /// <summary>
    /// El usuario tiene el tipo cliente.
    /// </summary>
    Cliente = 1,

    /// <summary>
    /// El usuario es un empleado de la empresa.
    /// </summary>
    Empleado = 2,

    /// <summary>
    /// El usuario es un empleado de un cliente.
    /// </summary>
    EmpleadoCliente = 3,

    /// <summary>
    /// Son usuarios de un cliente que no hacen login, se autetican con un token
    /// permanente de acceso.
    /// </summary>
    LoginLessCliente = 4
}

/// <summary>
/// Esatdo de la cuenta.
/// </summary>
public enum EstadoCuenta
{
    /// <summary>
    /// El suaurio se ha registrado pero no ha confirmado la cuenta.
    /// </summary>
    PendienteConfirmacion = 0,

    /// <summary>
    /// El uuario se encuentra activo.
    /// </summary>
    Activo = 1,

    /// <summary>
    /// El uuario se encuentra inactivo.
    /// </summary>
    Inactivo = 2,

    /// <summary>
    /// La cuenta se encuentra en proceso de baja.
    /// </summary>
    BajaCliente = 3,
}
