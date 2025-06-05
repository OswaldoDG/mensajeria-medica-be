using System.Net;
using comunes.busqueda;
using comunes.extensiones;
using comunes.respuestas;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.services.identity;
using mensajeriamedica.services.identity.administracion;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.model.identity.administracion;

/// <summary>
/// Implementacion del servicio de administracion.
/// </summary>
public class ServicioAdministracion(ILogger<ServicioAdministracion> logger, DbContextIdentity db,
    UserManager<ApplicationUser> userManager) : IServicioAdministracion
{

    /// <summary>
    /// Devuelve una lista de clientes.
    /// </summary>
    /// <param name="busqueda">Parámetros de búsqueda.</param>
    /// <param name="usuarioId">Usuario de ejecución.</param>
    /// <returns>Resultados de la búsqueda.</returns>
    public async Task<RespuestaPayload<ResultadoPaginado<CuentaCliente>>> BuscaClientes(Busqueda busqueda, Guid usuarioId)
    {
        logger.LogDebug("ServicioAdministracion-BuscaClientes {Busqueda} @{Usuario}", busqueda, usuarioId);

        RespuestaPayload<ResultadoPaginado<CuentaCliente>> respuesta = new ();

        try
        {
            ServicioBusquedaSQL<ApplicationUser> servicio = new ServicioBusquedaSQL<ApplicationUser>(DbContextIdentity.TABLAUSUARIO);

            busqueda.Filtros = busqueda.Filtros.Where(f => f.Propiedad != "TipoCuenta").ToList();
            busqueda.Filtros.Add(new Filtro() { Propiedad = "TipoCuenta", Operador = Operador.Igual, Valores = [TipoCuenta.Cliente.ToString()] });

            var pagina = await servicio.Buscar(busqueda, db);

            ResultadoPaginado<CuentaCliente> resultadoCuentasCliente = new ()
            {
                Contar = busqueda.Contar,
                Filtros = busqueda.Filtros,
                OrdenarPropiedad = busqueda.OrdenarPropiedad,
                OrdernarDesc = busqueda.OrdernarDesc,
                Total = pagina.Total,
                Paginado = busqueda.Paginado,
                Elementos = []
            };

            foreach (var usuario in pagina.Elementos)
            {
                var empleado = new CuentaCliente()
                {
                    Email = usuario.Email!,
                    Estado = usuario.Estado,
                    FechaActivacion = usuario.FechaActivacion,
                    FechaRegistro = usuario.FechaRegistro,
                    Id = Guid.Parse(usuario.Id),
                    Nombre = usuario.Nombre
                };
                resultadoCuentasCliente.Elementos.Add(empleado);
            }

            respuesta.Payload = resultadoCuentasCliente;
            respuesta.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioAdministracion-BuscaClientes Error al realizar la búsqueda {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    /// <summary>
    /// Devuelve una lista de empleados.
    /// </summary>
    /// <param name="busqueda">Parámetros de búsqueda.</param>
    /// <param name="usuarioId">Usuario de ejecución.</param>
    /// <returns>Resultados de la búsqueda.</returns>
    public async Task<RespuestaPayload<ResultadoPaginado<CuentaEmpleado>>> BuscaEmpleados(Busqueda busqueda, Guid usuarioId)
    {
        logger.LogDebug("ServicioAdministracion-BuscaEmpleados {Busqueda} @{Usuario}", busqueda, usuarioId);
        RespuestaPayload<ResultadoPaginado<CuentaEmpleado>> respuesta = new ();

        try
        {
            ServicioBusquedaSQL<ApplicationUser> servicio = new ServicioBusquedaSQL<ApplicationUser>(DbContextIdentity.TABLAUSUARIO);
            busqueda.Filtros = busqueda.Filtros.Where(f => f.Propiedad != "TipoCuenta").ToList();
            busqueda.Filtros.Add(new Filtro() { Propiedad = "TipoCuenta", Operador = Operador.Igual, Valores = [TipoCuenta.Empleado.ToString()] });

            var pagina = await servicio.Buscar(busqueda, db);

            ResultadoPaginado<CuentaEmpleado> resultadoCuentaEmpleado = new ()
            {
                Contar = busqueda.Contar,
                Filtros = busqueda.Filtros,
                OrdenarPropiedad = busqueda.OrdenarPropiedad,
                OrdernarDesc = busqueda.OrdernarDesc,
                Total = pagina.Total,
                Paginado = busqueda.Paginado,
                Elementos = []
            };

            foreach (var usuario in pagina.Elementos)
            {
                var empleado = new CuentaEmpleado()
                {
                    Email = usuario.Email!,
                    Estado = usuario.Estado,
                    FechaActivacion = usuario.FechaActivacion,
                    FechaRegistro = usuario.FechaRegistro,
                    Id = Guid.Parse(usuario.Id),
                    Nombre = usuario.Nombre,
                };
                resultadoCuentaEmpleado.Elementos.Add(empleado);
            }

            respuesta.Payload = resultadoCuentaEmpleado;
            respuesta.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioAdministracion-BuscaEmpleados Error al realizar la búsqueda {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    /// <summary>
    /// Crea un empleado.
    /// </summary>
    /// <param name="empleado">Datos de creación.</param>
    /// <returns>Empledo creado o error.</returns>
    public async Task<RespuestaPayload<CuentaEmpleado>> CreaEmpleado(CrearEmpleado empleado)
    {
        RespuestaPayload<CuentaEmpleado> respuesta = new ();

        var user = await userManager.FindByEmailAsync(empleado.Email);
        if (user is not null)
        {
            respuesta.Error = new ()
            {
                HttpCode = HttpStatusCode.Conflict,
                Codigo = CodigosError.IDENTITY_SERVICIO_ADMINISTRADOR_EMPLEADO_YA_REGISTRADO,
                Mensaje = "El Email proporcionado ya se encuentra registrado"
            };
            respuesta.HttpCode = HttpStatusCode.Conflict;
        }

        var usuario = new ApplicationUser()
        {
            Nombre = empleado.Nombre,
            UserName = empleado.Email,
            Email = empleado.Email,
            CuentaFiscalId = empleado.CuentaFiscalId
        };

        var result = await userManager.CreateAsync(usuario, empleado.Password);
        if (!result.Succeeded)
        {
            respuesta.Error = new ()
            {
                HttpCode = HttpStatusCode.InternalServerError,
                Codigo = CodigosError.IDENTITY_ERROR_DESCONOCIDO,
                Mensaje = "Ocurrió un problema inesperado en la creación de la cuenta del empleado"
            };
            respuesta.HttpCode = HttpStatusCode.InternalServerError;
        }

        respuesta.Ok = true;
        return respuesta;
    }

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    /// <param name="actualizaContrasena">Datos de actualziacion.</param>
    /// <param name="usuarioId">Identificador del usuario.</param>
    /// <returns>Resoltado o error de la operación.</returns>
    public async Task<Respuesta> CambiaContrasenaEmpleado(ActualizaContrasenaEmpleado actualizaContrasena, Guid usuarioId)
    {
        Respuesta r = new ();
        try
        {
            var empleado = await userManager.FindByIdAsync(actualizaContrasena.EmpleadoId.ToString());
            if (empleado is null)
            {
                r.Error = new ()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_ADMINISTRADOR_EMPLEADO_NO_ENCONTRADO,
                    Mensaje = "Empleado no encontrado para realizar la actualización del estado de la cuenta",
                    HttpCode = HttpStatusCode.NotFound,
                };
                r.HttpCode = HttpStatusCode.NotFound;
                return r;
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(empleado);

            var resetPassword = await userManager.ResetPasswordAsync(empleado, token, actualizaContrasena.NuevoPassword);

            if (!resetPassword.Succeeded)
            {
                r.Error = new ()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_ADMINISTRADOR_ERROR_CONTRASENA_EMPLEADO,
                    Mensaje = "Ocurrió un error inesperado en la actualización de la contraseña del empleado",
                    HttpCode = HttpStatusCode.InternalServerError,
                };
                r.HttpCode = HttpStatusCode.InternalServerError;
                return r;
            }

            r.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioAdministracion-CambiaContrasenaEmpleado Error al realizar la operación {Mensaje}", ex.Message);
            r.Error = ex.ToError();
        }

        return r;
    }

    /// <summary>
    ///  Establece el estado de la cuenta de un empleado.
    /// </summary>
    /// <param name="empleadoId">Id del empleado.</param>
    /// <param name="estado">nuevo estado de empleado.</param>
    /// <param name="usuarioId">Id del usuaurio de ejecución</param>
    /// <returns>Respuesta del proceso.</returns>
    public async Task<Respuesta> EstadoEmpleado(Guid empleadoId, EstadoCuenta estado, Guid usuarioId)
    {
        Respuesta r = new ();
        try
        {
            var empleado = await userManager.FindByIdAsync(empleadoId.ToString());
            if (empleado is null)
            {
                r.Error = new ()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_ADMINISTRADOR_EMPLEADO_NO_ENCONTRADO,
                    Mensaje = "Empleado no encontrado para realizar la actualización del estado de la cuenta",
                    HttpCode = HttpStatusCode.NotFound,
                };
                r.HttpCode = HttpStatusCode.NotFound;
                return r;
            }

            empleado.Estado = estado;
            var updateUser = await userManager.UpdateAsync(empleado);
            if (!updateUser.Succeeded)
            {
                r.Error = new ()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_ADMINISTRADOR_ERROR_CONTRASENA_EMPLEADO,
                    Mensaje = "Ocurrió un error inesperado en la actualización de la contraseña del empleado",
                    HttpCode = HttpStatusCode.InternalServerError,
                };
                r.HttpCode = HttpStatusCode.InternalServerError;
                return r;
            }

            r.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioAdministracion-EstadoEmpleado Error al realizar la operación {Mensaje}", ex.Message);
            r.Error = ex.ToError();
        }

        return r;
    }

    /// <summary>
    ///  Establece el estado de la cuenta de un empleado.
    /// </summary>
    /// <param name="clienteId">Id del empleado.</param>
    /// <param name="estado">nuevo estado de empleado.</param>
    /// <param name="usuarioId">Id del usuaurio de ejecución</param>
    /// <returns>Respuesta del proceso.</returns>
    public async Task<Respuesta> EstadoCliente(Guid clienteId, EstadoCuenta estado, Guid usuarioId)
    {
        Respuesta r = new ();

        try
        {
            var cliente = await userManager.FindByIdAsync(clienteId.ToString());
            if (cliente is null)
            {
                r.Error = new ()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_ADMINISTRADOR_EMPLEADO_NO_ENCONTRADO,
                    Mensaje = "Cliente no encontrado para realizar la actualización del estado de la cuenta",
                    HttpCode = HttpStatusCode.NotFound,
                };
                r.HttpCode = HttpStatusCode.NotFound;
                return r;
            }

            cliente.Estado = estado;
            var userUpdate = await userManager.UpdateAsync(cliente);
            if (!userUpdate.Succeeded)
            {
                r.Error = new ()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_ADMINISTRADOR_ERROR_ACTUALIZACION_ESTADO_CLIENTE,
                    Mensaje = "Ocurrió un error inesperado en la actualización de la contraseña del cliente",
                    HttpCode = HttpStatusCode.InternalServerError,
                };
                r.HttpCode = HttpStatusCode.InternalServerError;
                return r;
            }

            r.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioAdministracion-EstadoCliente Error al realizar la operación {Mensaje}", ex.Message);
            r.Error = ex.ToError();
        }

        return r;
    }
 
}
