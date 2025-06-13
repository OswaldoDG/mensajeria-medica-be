namespace mensajeriamedica.model.identity.registro;


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
