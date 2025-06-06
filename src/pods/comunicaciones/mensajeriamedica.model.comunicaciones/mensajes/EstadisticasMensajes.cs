namespace mensajeriamedica.model.comunicaciones.mensajes;

/// <summary>
/// Clase que representa las estadísticas de mensajes enviados.
/// </summary>
public class EstadisticasMensajes
{
    /// <summary>
    /// Identificador del registro de estadística de mensajes.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del registro de estadística de mensajes.
    /// </summary>
    public long ServidorId { get; set; }

    /// <summary>
    /// Identificador de la sucursal si existe.
    /// </summary>
    public string? SucursalId { get; set; } // indexar, maxlen 100

    /// <summary>
    /// Fecha año de la estadística de mensajes.
    /// </summary>
    public int Ano { get; set; } = 0;

    /// <summary>
    /// Fecha mes de la estadística de mensajes.
    /// </summary>
    public int Mes { get; set; } = 0;

    /// <summary>
    /// Fecha día de la estadística de mensajes.
    /// </summary>
    public int Dia { get; set; } = 0;

    /// <summary>
    /// Número de mensajes procesados exitosamente.
    /// </summary>
    public int Procesados { get; set; } = 0;

    /// <summary>
    /// Número de mensajes enviados satisfactoriamente.
    /// </summary>
    public int Enviados { get; set; } = 0;

    /// <summary>
    /// Número de mensajes erroneos recibidos en la carpeta del servidor.
    /// </summary>
    public int Erroneos { get; set; } = 0;

    /// <summary>
    /// Número de mensajes duplicados recibidos en la carpeta del servidor.
    /// </summary>
    public int Duplicados { get; set; } = 0;
}
