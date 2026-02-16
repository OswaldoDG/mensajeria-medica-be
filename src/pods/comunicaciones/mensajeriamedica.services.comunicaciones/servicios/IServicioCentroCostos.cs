using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.centroscostos;
using mensajeriamedica.model.comunicaciones.centroscostos.dtos;

namespace mensajeriamedica.services.comunicaciones.servicios;

/// <summary>
/// Operaciones del centro de costos.
/// </summary>
public interface IServicioCentroCostos
{
    Task<RespuestaPayload<List<CentroCostos>>> ObtieneCentroCostos(bool incluirEliminados);
    Task<RespuestaPayload<CentroCostos>> CreaCentroCostos(string nombre);
    Task<Respuesta> ActualizaCentroCostos(int id, string nombre);

    /// <summary>
    /// MArca el centro de costos como eliminado.
    /// </summary>
    /// <param name="id">Id del centro de costos.</param>
    /// <returns>Resultado.</returns>
    Task<Respuesta> EliminaCentroCostos(int id);

    Task<RespuestaPayload<UnidadCostos>> CreaUnidadCostos(int centroCostosId, string nombre, string clave);
    Task<Respuesta> ActualizaUnidadCostos(int centroCostosId, int id, string nombre, string clave);

    /// <summary>
    /// Sólo deben oderse eliminar unidades si no hay mensajes con su clave en el campo SucursalId.
    /// </summary>
    /// <param name="id">Id de la unidad.</param>
    /// <returns>Resultado.</returns>
    Task<Respuesta> EliminaUnidadCostos(int centroCostosId, int id);

    Task<Respuesta> AgregaUsuarioCentroCostos(int centroCostosId, Guid UsuarioID);

    Task<Respuesta> EliminaUsuarioCentroCostos(int centroCostosId, Guid UsuarioID);
    Task<RespuestaPayload<List<DtoUsuario>>> ObtieneListaUsuarios();
}
