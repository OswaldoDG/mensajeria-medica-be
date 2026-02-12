namespace mensajeriamedica.model.comunicaciones.centroscostos;

public class UnidadCostosDto
{
    /// <summary>
    /// NOmbre de la unidad por ejemplo Gabinete, laboratorio, etc.
    /// </summary>
    required public string Nombre { get; set; }

    /// <summary>
    /// Clave de la unidad, es la que se utiliza en los mensajes como SucursalId.
    /// </summary>
    required public string Clave { get; set; }
}
