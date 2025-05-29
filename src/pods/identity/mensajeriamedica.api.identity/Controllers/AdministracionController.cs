using comunes;
using comunes.auth;
using comunes.busqueda;
using comunes.respuestas;
using comunes.servicios.crm;
using mensajeriamedica.model.identity.administracion;
using mensajeriamedica.services.identity.administracion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;

namespace mensajeriamedica.api.identity.Controllers;

[Route("administracion")]
[ApiController]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class AdministracionController(ILogger<AdministracionController> logger, IServicioAdministracion servicioAdministracion, IServicioInterprocesoSesion servicioSesion) : ControllerUsoInterno(logger)
{
    [Authorize(Roles = "AdminUsuarios,AdminSistema,AdminUsuariosEx")]
    [SwaggerOperation("Crea un empleado")]
    [SwaggerResponse(statusCode: 200, description: "Empleado creado correctamente")]
    [SwaggerResponse(statusCode: 400, description: "Datos incorrectos para la creación")]
    [HttpPost("empleados/crear")]
    public async Task<ActionResult<RespuestaPayload<CuentaEmpleado>>> CreaEmpleado([FromBody] CrearEmpleado crearEmpleado)
    {
        bool esAdminExterno = false;
        bool esAdminInterno = false;

        // Si el empelado es creado relacioando con una CuentaFiscalId es necesario que el creador tenga el rol adecuado o sea la cuenta primaria.
        if (crearEmpleado.CuentaFiscalId != null)
        {
            esAdminExterno = await servicioSesion.EsPrimarioCuentaFiscal(crearEmpleado.CuentaFiscalId!.Value, UsuarioGuid!.Value);
            if (!esAdminExterno)
            {
                esAdminExterno = false; // await servicioSesion.ExisteUsuarioCuentaFiscal(crearEmpleado.CuentaFiscalId!.Value, UsuarioGuid!.Value, [ConstantesSeguridad.RolAdminUsuarios]);
            }
        }
        else
        {
            // Si el empleado es de contabee solo debe tenrese el rol adecuado.
            esAdminInterno = false; // TieneRolesOr([ConstantesSeguridad.RolAdminUsuarios, ConstantesSeguridad.RolAdminSistema]);
        }

        if (!esAdminExterno && !esAdminInterno)
        {
            logger.LogDebug("AdministradorController - rol no valido");
            return Forbid();
        }

        logger.LogDebug("AdministradorController - CreaEmpleado");
        var respuesta = await servicioAdministracion.CreaEmpleado(crearEmpleado);
        if (respuesta is null)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
        }

        logger.LogDebug("AdministradorController - Empleado Creado");
        return Ok(respuesta);
    }

    [Authorize(Roles = "AdminUsuarios,AdminSistema,AdminUsuariosEx")]
    [SwaggerOperation("Busca Empleados")]
    [SwaggerResponse(statusCode: 200, description: "Empleado Encontrado")]
    [SwaggerResponse(statusCode: 404, description: "Empleado No Encontrado")]
    [HttpPost("empleados/buscar")]
    public async Task<ActionResult<ResultadoPaginado<CuentaCliente>>> BuscaEmpleados([FromBody] Busqueda busqueda, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        logger.LogDebug("AdministracionController - BuscaEmpleados");

        if (cuentaFiscalId == null)
        {
            return BadRequest("El parametro de querystring 'cfid' es necesario para administradores externos");
        }
        else
        {
            var usuarioValido = await servicioSesion.ExisteUsuarioCuentaFiscal(cuentaFiscalId!.Value, UsuarioGuid!.Value, null);
            if (!usuarioValido)
            {
                return BadRequest($"El usuario no pertenece a la cuenta fiscal {cuentaFiscalId}");
            }
            else
            {
                busqueda.Filtros.Add(new Filtro() { Propiedad = "CuentaFiscalId", Operador = Operador.Igual, Valores = [cuentaFiscalId.ToString()] });
            }
        }

        var respuesta = await servicioAdministracion.BuscaEmpleados(busqueda, UsuarioGuid!.Value);
        if (respuesta is null)
        {
            return ActionFromCode(respuesta!.HttpCode, respuesta.Error!.Mensaje);
        }

        logger.LogDebug("AdministracionController - BuscaEmpleados {Conteo} elementos encontrados", respuesta.Payload!.Elementos.Count);
        return Ok(respuesta);
    }

    [Authorize(Roles = "AdminUsuarios,AdminSistema,AdminUsuariosEx")]
    [SwaggerOperation("Cambia la contraseña de un empleado")]
    [SwaggerResponse(statusCode: 200, description: "Proceso Exitoso - Contraseña Actualizada")]
    [SwaggerResponse(statusCode: 400, description: "Datos incorrectos para realizar la actualizacion de la contraseña")]
    [HttpPut("usuarios/contrasena")]
    public async Task<ActionResult<Respuesta>> CambioContrasenaEmpleado([FromBody] ActualizaContrasenaEmpleado actualizaContrasena, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        logger.LogDebug("AdministradorController - CambioContrasenaEmpleado");

        // Para los administardores externos es necesario incluir la cuenta fiscal.
        if (cuentaFiscalId == null)
        {
            return BadRequest("El parametro de querystring 'cfid' es necesario para administradores externos");
        }
        else
        {
            var usuarioValido = await servicioSesion.ExisteUsuarioCuentaFiscal(cuentaFiscalId!.Value, UsuarioGuid!.Value, null);
            var empleadoValido = await servicioSesion.ExisteUsuarioCuentaFiscal(cuentaFiscalId!.Value, actualizaContrasena.EmpleadoId, null);
            if (!usuarioValido || !empleadoValido)
            {
                return BadRequest($"Las identidades no pertenecen a la cuenta fiscal {cuentaFiscalId}");
            }
        }

        var respuesta = await servicioAdministracion.CambiaContrasenaEmpleado(actualizaContrasena, UsuarioGuid!.Value!);
        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error!.Mensaje);
        }

        return Ok(respuesta);
    }

    [Authorize(Roles = "AdminUsuarios,AdminSistema,AdminUsuariosEx")]
    [SwaggerOperation("Actualiza el estado del empleado")]
    [SwaggerResponse(statusCode: 200, description: "Estado del empleado actualizdo con éxito")]
    [SwaggerResponse(statusCode: 400, description: "Datos incorrectos para la actualizacion")]
    [HttpPut("empleados/{empleadoId}/estado/{nuevoEstado}")]
    public async Task<ActionResult<Respuesta>> EstadoEmpleado(Guid empleadoId, model.identity.registro.EstadoCuenta nuevoEstado, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        logger.LogDebug("AdministradorController - EstadoEmpleado");

        if (cuentaFiscalId == null)
        {
            return BadRequest("El parametro de querystring 'cfid' es necesario para administradores externos");
        }
        else
        {
            var usuarioValido = await servicioSesion.ExisteUsuarioCuentaFiscal(cuentaFiscalId!.Value, UsuarioGuid!.Value, null);
            var empleadoValido = await servicioSesion.ExisteUsuarioCuentaFiscal(cuentaFiscalId!.Value, empleadoId, null);
            if (!usuarioValido || !empleadoValido)
            {
                return BadRequest($"Las identidades no pertenecen a la cuenta fiscal {cuentaFiscalId}");
            }
        }

        var respuesta = await servicioAdministracion.EstadoEmpleado(empleadoId, nuevoEstado, UsuarioGuid!.Value);
        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error!.Mensaje);
        }

        logger.LogDebug("AdministradorController - Estado del Empleado actualizado");
        return Ok(respuesta);
    }

    [Authorize(Roles = "AdminUsuarios,AdminSistema")]
    [SwaggerOperation("Busca Clientes")]
    [SwaggerResponse(statusCode: 200, description: "Cliente Encontrado")]
    [SwaggerResponse(statusCode: 404, description: "Cliente No Encontrado")]
    [HttpPost("clientes/buscar")]
    public async Task<ActionResult<ResultadoPaginado<CuentaCliente>>> BuscaClientes(Busqueda busqueda)
    {
        logger.LogDebug("AdministracionController - BuscaClientes");
        var respuesta = await servicioAdministracion.BuscaClientes(busqueda, UsuarioGuid!.Value);
        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error!.Mensaje);
        }

        logger.LogDebug("AdministracionController - BuscaClientes {Conteo} elementos encontrados", respuesta.Payload!.Elementos.Count);
        return Ok(respuesta);
    }

    [Authorize(Roles = "AdminUsuarios,AdminSistema")]
    [SwaggerOperation("Actualiza el estado del cliente")]
    [SwaggerResponse(statusCode: 200, description: "Estado del Cliente actualizado con éxito")]
    [SwaggerResponse(statusCode: 400, description: "Datos incorrectos para la actualizacion")]
    [HttpPut("clientes/{clienteId}/estado/{nuevoEstado}")]
    public async Task<ActionResult<Respuesta>> EstadoCliente(Guid clienteId, model.identity.registro.EstadoCuenta nuevoEstado)
    {
        logger.LogDebug("AdministradorController - EstadoCliente");
        var respuesta = await servicioAdministracion.EstadoCliente(clienteId, nuevoEstado, UsuarioGuid!.Value);
        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error!.Mensaje);
        }

        logger.LogDebug("AdministradorController - Estado del Cliente actualizado");
        return Ok(respuesta);
    }
}