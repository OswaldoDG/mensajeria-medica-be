namespace mensajeriamedica.model.comunicaciones.mensajes;

public class DtoMensaje
{
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
    /// <summary>
    /// Identificador del mensaje.
    /// </summary>
    public long Id { get; set; } // Clave primaria.

    /// <summary>
    /// FEcha de creación del mensaje.
    /// </summary>
    public DateTime FechaCreacion { get; set; } // indexar

    /// <summary>
    /// Estado del mensaje.
    /// </summary>
    public EstadoMensaje Estado { get; set; } // indexar

    /// <summary>
    /// Teléfono del contacto.
    /// </summary>
    public string Telefono { get; set; } // maxlen 15

    /// <summary>
    /// Nombre del cotnacto.
    /// </summary>
    public string NombreContacto { get; set; } // maxlen 100

    /// <summary>
    /// URL para la consulta del estudio.
    /// </summary>
    public string Url { get; set; } // maxlen 300

    /// <summary>
    /// Identificador del servidor al que pertenece el mensaje.
    /// </summary>
    public long ServidorId { get; set; }

    /// <summary>
    /// Identificador de la sucursal que publica el mensaje.
    /// </summary>
    public string SucursalId { get; set; } // indexas, maxlen 100
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
}
