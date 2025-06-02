using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.identity.administracion;
using mensajeriamedica.model.identity.registro;

namespace mensajeriamedica.services.identity.administracion;

/// <summary>
/// Defincion del servicio de adminsitracion.
/// </summary>
public interface IServicioAdministracion
{
    /// <summary>
    /// Devuelve una lista de clientes.
    /// </summary>
    /// <param name="busqueda">Parámetros de búsqueda.</param>
    /// <param name="usuarioId">Usuario de ejecución.</param>
    /// <returns>Resultados de la búsqueda.</returns>
    Task<RespuestaPayload<ResultadoPaginado<CuentaCliente>>> BuscaClientes(Busqueda busqueda, Guid usuarioId);

    /// <summary>
    /// Devuelve una lista de empleados.
    /// </summary>
    /// <param name="busqueda">Parámetros de búsqueda.</param>
    /// <param name="usuarioId">Usuario de ejecución.</param>
    /// <returns>Resultados de la búsqueda.</returns>
    Task<RespuestaPayload<ResultadoPaginado<CuentaEmpleado>>> BuscaEmpleados(Busqueda busqueda, Guid usuarioId);

    /// <summary>
    /// Cambia la contraseña de un empleado.
    /// </summary>
    /// <param name="actualizaContrasena">Datos de actualización.</param>
    /// <param name="usuarioId">Id del usuaurio de ejecución</param>
    /// <returns>Respuesta del proceso.</returns>
    Task<Respuesta> CambiaContrasenaEmpleado(ActualizaContrasenaEmpleado actualizaContrasena, Guid usuarioId);

    /// <summary>
    ///  Establece el estado de la cuenta de un empleado.
    /// </summary>
    /// <param name="clienteId">Id del empleado.</param>
    /// <param name="estado">nuevo estado de empleado.</param>
    /// <param name="usuarioId">Id del usuaurio de ejecución</param>
    /// <returns>Respuesta del proceso.</returns>
    Task<Respuesta> EstadoCliente(Guid clienteId, EstadoCuenta estado, Guid usuarioId);

    /// <summary>
    ///  Establece el estado de la cuenta de un empleado.
    /// </summary>
    /// <param name="empleadoId">Id del empleado.</param>
    /// <param name="estado">nuevo estado de empleado.</param>
    /// <param name="usuarioId">Id del usuaurio de ejecución</param>
    /// <returns>Respuesta del proceso.</returns>
    Task<Respuesta> EstadoEmpleado(Guid empleadoId, EstadoCuenta estado, Guid usuarioId);

    /// <summary>
    /// Crea un empleado.
    /// </summary>
    /// <param name="empleado">Datos de creación</param>
    /// <returns>Empledo creado.</returns>
    Task<RespuestaPayload<CuentaEmpleado>> CreaEmpleado(CrearEmpleado empleado);
}
