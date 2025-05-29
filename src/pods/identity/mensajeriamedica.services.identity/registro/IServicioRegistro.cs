using comunes.respuestas;
using mensajeriamedica.model.identity.registro;

namespace contabee.services.identity.registro;

/// <summary>
/// Métodos del serviciso de regsitro de usuarios.
/// </summary>
public interface IServicioRegistro
{
    /// <summary>
    /// Crea una solicitud de registro.
    /// </summary>
    /// <param name="registro">Datos de registro.</param>
    /// <returns>Respuesta del proceso</returns>
    Task<Respuesta> Registro(RegisterViewModel registro);

    /// <summary>
    /// Devuelve los datos asociados a un registro.
    /// </summary>
    /// <param name="id">Identificdo del regsitro.</param>
    /// <returns>Datos de registro o error asociado.</returns>
    Task<RespuestaPayload<DatosRegistro>> GetRegistroConfirmar(string id);

    /// <summary>
    /// Complet una operación de registro.
    /// </summary>
    /// <param name="id">Id del proceos.</param>
    /// <returns>Resultado o error asociado</returns>
    Task<Respuesta> PostRegistroConfirmar(string id);
}
