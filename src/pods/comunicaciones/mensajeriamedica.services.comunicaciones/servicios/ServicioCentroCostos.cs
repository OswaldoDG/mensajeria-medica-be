using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.centroscostos;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.servicios;

public class ServicioCentroCostos(ILogger<ServicioCentroCostos> logger, DbContextMensajeria db) : IServicioCentroCostos
{

    public Task<RespuestaPayload<CentroCostos>> CreaCentroCostos(string nombre)
    {
        // Verificar si no existe un homónimo activo, si existe devolver Conflict.
        throw new NotImplementedException();
    }

    public Task<Respuesta> ActualizaCentroCostos(int id, string nombre)
    {
        if (CentroCostosActivo(id))
        {
            // Verificar si no existe un homónimo activo, si existe devolver Conflic con Id diferente al que se intenta actualizar.
            // Actualizar sólo el nombre del centro de costos
        }

        throw new NotImplementedException();
    }

    public Task<Respuesta> ActualizaUnidadCostos(int centroCostosId, int id, string nombre, string clave)
    {
        if (CentroCostosActivo(id))
        {
            // VErificar si hay algun registro con la clave antigua en Mensajes.SucursalId
            // y actualizarlo con la nueva clave SOLO SI NO EXISTEN en la tabla mensajes en el campo.
            // Si hay algiun registro devolver Conflict.

            // Actualizar sólo el nombre y clave de la unidad de costos.
        }
        throw new NotImplementedException();
    }

    public Task<Respuesta> AgregaUsuarioCentroCostos(int centroCostosId, Guid UsuarioID)
    {
        if (CentroCostosActivo(centroCostosId))
        {
            // Realizar la operacón solo si el centro de costos está activo.
        }

        throw new NotImplementedException();
    }

    public Task<Respuesta> EliminaUsuarioCentroCostos(int centroCostosId, Guid UsuarioID)
    {
        if (CentroCostosActivo(centroCostosId))
        {
            // Realizar la operacón solo si el centro de costos está activo.
        }

        throw new NotImplementedException();
    }

    public Task<RespuestaPayload<UnidadCostos>> CreaUnidadCostos(int centroCostosId, string nombre, string clave)
    {
        if (CentroCostosActivo(centroCostosId))
        {
            // Realizar la operacón solo si el centro de costos está activo.
            // VErifica que no haya unidads con CLAVE IDENTICA, si pueden existir con el mismo nombre pero no con la misma clave, si existe devolver Conflict.
        }
        throw new NotImplementedException();
    }

    public Task<Respuesta> EliminaCentroCostos(int id)
    {
        // MArca Eliminado = true.
        throw new NotImplementedException();
    }

    public Task<Respuesta> EliminaUnidadCostos(int centroCostosId, int id)
    {
        if (CentroCostosActivo(centroCostosId))
        {
            // Realizar la operacón solo si el centro de costos está activo.
            // Puede eliminarla solo si no hay Mensajes.SucursalId con la clave de la unidad de costos, si hay algiun registro devolver Conflict.
        }
        throw new NotImplementedException();
    }

    /// <summary>
    /// VErifica si el centro de costos se ecuentra activo.
    /// </summary>
    /// <param name="id">ID del centro de costos.</param>
    /// <returns>Si el centro de costos está activo.</returns>
    private bool CentroCostosActivo(int id)
    {
        // Si el centro por id existe y no esta Eliminado devolver true.
        return false;
    }

    public Task<RespuestaPayload<List<CentroCostos>>> ObtieneCentroCostos(bool incluirEliminados)
    {
        // Devuelve la lista de centros de costos, si incluirEliminados es false sólo devuelve los que Eliminado = false, si incluirEliminados es true devuelve todos.
        throw new NotImplementedException();
    }
}
