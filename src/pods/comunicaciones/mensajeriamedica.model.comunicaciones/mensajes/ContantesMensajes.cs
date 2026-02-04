namespace mensajeriamedica.model.comunicaciones.mensajes;

/// <summary>
/// Estados del mensaje.
/// </summary>
public enum EstadoMensaje
{
    /// <summary>
    /// Mensaje pendiente de envío.
    /// </summary>
    Pendiente,

    /// <summary>
    /// Mensaje enviado correctamente.
    /// </summary>
    Enviado,

    /// <summary>
    /// Mensaje fallido al enviar.
    /// </summary>
    Fallido,

    /// <summary>
    /// Formato incorrecto en el archivo recibido.
    /// </summary>
    FormatoIncorrecto,

    /// <summary>
    /// Archivo duplciado
    /// </summary>
    Duplicado,

    /// <summary>
    /// FAllido al envia rpowhats
    /// </summary>
    FallidoWhatsApp, 
    
    /// <summary>
    /// NUmero de tgelñefono o valido
    /// </summary>
    NumTelInvalido
}