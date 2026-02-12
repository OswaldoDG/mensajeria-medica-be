using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mensajeriamedica.model.comunicaciones.centroscostos;

public class UnidadCostos
{
    /// <summary>
    /// Identificador único de la unidad.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Centro de costos al que pertenece la unidad.
    /// </summary>
    public int CentroCostosId { get; set; }

    /// <summary>
    /// NOmbre de la unidad por ejemplo Gabinete, laboratorio, etc.
    /// </summary>
    required public string Nombre { get; set; }
    // MAXLEN 200

    /// <summary>
    /// Clave de la unidad, es la que se utiliza en los mensajes como SucursalId.
    /// </summary>
    required public string Clave { get; set; }
    // MAXLEN 50

    /// <summary>
    /// Navegación.
    /// </summary>
    public CentroCostos CentroCostos { get; set; }
}
