namespace mensajeriamedica.model.comunicaciones.centroscostos;

/// <summary>
/// Relacion de usuario sque pertenecen a un centro de costos, un usuario puede pertenecer a varios centros de costos y un centro de costos puede tener varios usuarios.
/// </summary>
public class UsuarioCentroCostos
{
    public int Id { get; set; }
    public int CentroCostosId { get; set; }
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Navegación.
    /// </summary>
    public CentroCostos CentroCostos { get; set; }
}
