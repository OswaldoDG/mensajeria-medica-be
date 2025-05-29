using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.model.identity.usuarios;

namespace contabee.services.identity.usuarios;

/// <summary>
/// DEfine los metodos del servicio de usuarios.
/// </summary>
public interface IServicioUsuarios
{
    /// <summary>
    /// Obtiene los datos del perfil para la UI deun usuario.
    /// </summary>
    /// <param name="usuarioId">Identificador del usuario.</param>
    /// <returns>Perfil o errror.</returns>
    Task<RespuestaPayload<PerfilUsuario>> ObtienePerfil(Guid usuarioId);

    /// <summary>
    /// Crea un usuario de la aplicacion.
    /// </summary>
    /// <param name="datos">Datos de creacion.</param>
    /// <param name="usuarioCreadorId">Id del usuario creador.</param>
    /// <param name="cuentaFiscalId">Cunta fiscal en la que se creara el usuario.</param>
    /// <returns>Usuario creado o error.</returns>
    Task<RespuestaPayload<CuentaUsuario>> Crear(CreaUsuario datos, Guid usuarioCreadorId, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Crea un usuario de captura.
    /// </summary>
    /// <param name="datos">Datos de creacion.</param>
    /// <param name="usuarioCreadorId">Id del usuario creador.</param>
    /// <param name="cuentaFiscalId">Cunta fiscal en la que se creara el usuario.</param>
    /// <returns>Usuario creado o error.</returns>
    Task<RespuestaPayload<CuentaUsuario>> CrearUsuarioCaptura(CreaUsuarioCaptura datos, Guid usuarioCreadorId, Guid cuentaFiscalId);

    /// <summary>
    /// Actualzia los datos de un usuario.
    /// </summary>
    /// <param name="usuarioId">Identificador del usuario.</param>
    /// <param name="datos">Datos de actualizacion.</param>
    /// <param name="cuentaFiscalId">Cunat fiscal asociada.</param>
    /// <returns>Resultado del operacion.</returns>
    Task<Respuesta> Actualizar(Guid usuarioId, ActualizaUsuario datos, Guid? cuentaFiscalId = null);


    /// <summary>
    /// Busca usuarios en base a un criterio.
    /// </summary>
    /// <param name="busqueda">Parametos de la busqueda.</param>
    /// <param name="cuentaFiscalId">Cunat fiscal para realizar la busqueda.</param>
    /// <returns>Resultado de la busqueda.</returns>
    Task<RespuestaPayload<ResultadoPaginado<CuentaUsuario>>> Buscar(Busqueda busqueda, Guid? cuentaFiscalId = null);


    /// <summary>
    /// Asigna un nuevo estado al usuario.
    /// </summary>
    /// <param name="usuarioId">Id del usuario.</param>
    /// <param name="estado">Nuevo estado.</param>
    /// <param name="cuentaFiscalId">Cunta fiscal a la que pertenece el usuario.</param>
    /// <returns>Resultado de la operacion.</returns>
    Task<Respuesta> EstableceEstado(Guid usuarioId, EstadoCuenta estado, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Cambia la contraena de un usuario, si se proporciona una contrasena anterior
    /// debe validarse que sea la vigente.
    /// </summary>
    /// <param name="usuarioId">Id del usuario.</param>
    /// <param name="contrasenaNueva">Nueva contrasena.</param>
    /// <param name="contrasenaAnterior">Contrasena anterior para validar.</param>
    /// <param name="cuentaFiscalId">Cunta fiscal a la que pertenece el usuario.</param>
    /// <returns>Resultado de la operacion.</returns>
    Task<Respuesta> CambiarContrasena(Guid usuarioId, string contrasenaNueva, string? contrasenaAnterior = null, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Elimina el usuario por su id.
    /// </summary>
    /// <param name="usuarioId">Id del usaurio.</param>
    /// <param name="cuentaFiscalId">Cunta fiscal a la que pertenece el usuario.</param>
    /// <returns>Resultado de la operacion.</returns>
    Task<Respuesta> Eliminar(Guid usuarioId, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Actualiza los roles de un usuario.
    /// </summary>
    /// <param name="usuarioId">Id del usuario.</param>
    /// <param name="roles">Lista de roles a establecer.</param>
    /// <param name="eliminarAnteriores">Si es true eliminalos roles existentes.</param>
    /// <param name="cuentaFiscalId">Especifica a que cuenta fiscal deben aplicarse los roles.</param>
    /// <returns>Resultado de la operacion.</returns>
    Task<Respuesta> EstablecerRoles(Guid usuarioId, List<string> roles, bool eliminarAnteriores = false, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Obtiene los roles de un usuario, si se incluye el parametro uenta fiscal son los roles de la cuenta.
    /// </summary>
    /// <param name="usuarioId">Id del usuario.</param>
    /// <param name="cuentaFiscalId">Especifica a que cuenta fiscal deben aplicarse los roles.</param>
    /// <returns>Resultado de la operacion.</returns>
    Task<RespuestaPayload<List<string>>> ObtieneRoles(Guid usuarioId, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Crea un token login less para un usuario.
    /// </summary>
    /// <param name="solicitudToken">Datos de la solicitus.</param>
    /// <param name="usuarioCreadorId">Id del usuario creador.</param>
    /// <param name="cuentaFiscalId">Cunta fiscal a la que pertenece el usuario.</param>
    /// <returns>Token de accesso login less o el resultado de la operacion.</returns>
    Task<Respuesta> CrearTokenLoginLess(SolictudTokenLoginless solicitudToken, Guid usuarioCreadorId, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Obtene el token loginless asignado al dispositivo del usuario.
    /// </summary>
    /// <param name="dispositivoId">Id del dispositivo.</param>
    /// <returns>Token de accesso login less o el resultado de la operacion.</returns>
    Task<RespuestaPayload<ResultadoTokenLoginLess>> ObtieneTokenLoginLess(string dispositivoId);

    /// <summary>
    /// Elimina tokens loginless.
    /// </summary>
    /// <param name="tokenId">Id del token.</param>
    /// <param name="cuentaFiscalId">Cunta fiscal a la que pertenece el usuario.</param>
    /// <returns>Resultado de la operacion.</returns>
    Task<Respuesta> EliminaTokenLoginLess(long tokenId, Guid? cuentaFiscalId = null);

    /// <summary>
    /// Valida si el token se encuentra activo para un usaurio.
    /// </summary>
    /// <param name="token">Token a comparar.</param>
    /// <returns>Respuesta de la operacion con true si el token es válido.</returns>
    Task<RespuestaPayload<ResultadoValidacionInternaToken>> ValidaTokenLoginLess(string token);

    /// <summary>
    /// Obtiene token de vinculación.
    /// </summary>
    /// <param name="DeviceId">Id del dispotivio.</param>
    /// <returns>Retorno de operación.</returns>
    Task<RespuestaPayload<RespuestaTokenVinculacion>> ObtieneTokenVinculacion(string DeviceId);

    /// <summary>
    /// Verifica la activación de un Token de vinculación.
    /// </summary>
    /// <param name="dispositivoId">Id del dispositivo.</param>
    /// <param name="token">Token de vinculación.</param>
    /// <returns>Retorno de operación.</returns>
    Task<Respuesta> VerificarActivacion(string dispositivoId, string token);

    /// <summary>
    /// Obtiene los usuarios asociados a una lista de ids.
    /// </summary>
    /// <param name="ids">Lista de ids.</param>
    /// <returns>Resultados.</returns>
    Task<RespuestaPayload<List<ParIdTexto>>> ObtieneUsuariosDeIds(List<string> ids);

    /// <summary>
    /// Genera el DTO para la recuperaciòn de la contraseña de un usuario.
    /// </summary>
    /// <param name="email">Correo del usuario.</param>
    /// <returns>Retorna el DTO para recuperar la contraseña.</returns>
    Task<RespuestaPayload<DTORecuperaContrasena>> RecuperaContrasenaEmail(string email);

    /// <summary>
    /// Establece contraseña.
    /// </summary>
    /// <param name="recuperaContrasena">DTO para recuperar la contraseña.</param>
    /// <returns>Retorno de operaciòn.</returns>
    Task<Respuesta> EstablaceContrasenaToken(ActualizaContrasena recuperaContrasena);
}
