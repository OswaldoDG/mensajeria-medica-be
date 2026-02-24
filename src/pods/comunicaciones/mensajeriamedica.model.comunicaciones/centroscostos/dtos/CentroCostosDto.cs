namespace mensajeriamedica.model.comunicaciones.centroscostos;

/// <summary>
/// El centro de costos es una unidad organizativa dentro de una empresa que se utiliza para asignar y controlar los costos asociados a una actividad, proyecto o departamento específico. En el contexto de la mensajería médica, un centro de costos podría representar un área específica de la organización, como un departamento de atención al cliente, un equipo de desarrollo de software o un proyecto específico relacionado con la mensajería médica. 
/// El centro de costos permite a la empresa realizar un seguimiento detallado de los gastos y asignar recursos de manera eficiente.
/// </summary>
public class CentroCostosDto
{
    /// <summary>
    /// Nombre del centro puedes ser por ejmeplo una persona o un hospital.
    /// </summary>
    public string Nombre { get; set; }
}
