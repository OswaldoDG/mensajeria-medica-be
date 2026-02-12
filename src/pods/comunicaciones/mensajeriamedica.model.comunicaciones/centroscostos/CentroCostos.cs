namespace mensajeriamedica.model.comunicaciones.centroscostos;

/// <summary>
/// El centro de costos es una unidad organizativa dentro de una empresa que se utiliza para asignar y controlar los costos asociados a una actividad, proyecto o departamento específico. En el contexto de la mensajería médica, un centro de costos podría representar un área específica de la organización, como un departamento de atención al cliente, un equipo de desarrollo de software o un proyecto específico relacionado con la mensajería médica. 
/// El centro de costos permite a la empresa realizar un seguimiento detallado de los gastos y asignar recursos de manera eficiente.
/// </summary>
public class CentroCostos
{
    /// <summary>
    /// Identificador único del centro.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del centro puedes ser por ejmeplo una persona o un hospital.
    /// </summary>
    public string Nombre { get; set; }
    // MAXLEN 200

    /// <summary>
    /// Especifica si el centro de costos se encuentra eliminado, La eliminación es virtua para poder tener estadisticas de los centros de costos eliminados, 
    /// pero no se pueden asignar unidades de costos ni usuarios a un centro de costos eliminado.
    /// </summary>
    public bool Eliminado { get; set; } = false;

    /// <summary>
    /// UNidades de costos asociadas al centro.
    /// </summary>
    public List<UnidadCostos> Unidades { get; set; } = [];

    /// <summary>
    /// Usuarios asociados al centro de costos, un usuario puede pertenecer a varios centros de costos y un centro de costos puede tener varios usuarios.
    /// </summary>
    public List<UsuarioCentroCostos> Usuarios { get; set; } = [];
}
