using comunes;
using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.model.identity.usuarios;
using mensajeriamedica.services.identity.usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;

namespace mensajeriamedica.api.identity.Controllers;

[Route("usuarios")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
public class UsuariosController(ILogger<UsuariosController> logger, IServicioUsuarios servicioUsuarios) : ControllerUsoInterno(logger)
{
    [SwaggerOperation("Crea un nuevo usuario")]
    [SwaggerResponse(statusCode: 200, type: typeof(CuentaUsuario), description: "Usuario Registrado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al crear usuario")]
    [SwaggerResponse(statusCode: 409, description: "El usuario ya existe")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPost]
    public async Task<ActionResult<CuentaUsuario>> Crear([FromBody] CreaUsuario datos, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        if (datos.CuentaFiscalId != cuentaFiscalId)
        {
            return BadRequest("Los datos de paylod de CuentaFiscalId no concuerdan");
        }

        var respuesta = await servicioUsuarios.Crear(datos, UsuarioGuid!.Value, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok(respuesta.Payload);
    }

    [SwaggerOperation("Crea un nuevo usuario de captura")]
    [SwaggerResponse(statusCode: 200, type: typeof(CuentaUsuario), description: "Usuario Registrado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al crear usuario")]
    [SwaggerResponse(statusCode: 409, description: "El usuario ya existe")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPost("captura")]
    public async Task<ActionResult<CuentaUsuario>> CrearUsuarioCaptura([FromBody] CreaUsuarioCaptura datos, [FromQuery(Name = "cfid")] Guid cuentaFiscalId)
    {
        if (datos.CuentaFiscalId != cuentaFiscalId)
        {
            return BadRequest("Los datos de paylod de CuentaFiscalId no concuerdan");
        }

        var respuesta = await servicioUsuarios.CrearUsuarioCaptura(datos, UsuarioGuid!.Value, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok(respuesta.Payload);
    }

    [SwaggerOperation("Actualiza un usuario")]
    [SwaggerResponse(statusCode: 200, description: "Usuario actualizado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al actualizar usuario")]
    [SwaggerResponse(statusCode: 404, description: "Usuario inexistente")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPut("{usuarioId}")]
    public async Task<ActionResult> Actualizar([FromRoute] Guid usuarioId, [FromBody] ActualizaUsuario datos, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        if (usuarioId != datos.UsuarioId)
        {
            return BadRequest("El usuario de la actualziación no corresponde con el query");
        }

        var respuesta = await servicioUsuarios.Actualizar(usuarioId, datos, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok();
    }

    [SwaggerOperation("Busca usuarios en base a un criterio")]
    [SwaggerResponse(statusCode: 200, description: "Pagina valida de resultados")]
    [SwaggerResponse(statusCode: 500, description: "Error al obtener los resultados")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPost("buscar")]
    public async Task<ActionResult<ResultadoPaginado<CuentaUsuario>>> Buscar([FromBody] Busqueda busqueda, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        var respuesta = await servicioUsuarios.Buscar(busqueda, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok(respuesta.Payload);
    }

    [SwaggerOperation("Actualiza la contraseña de un usuario.")]
    [SwaggerResponse(statusCode: 200, description: "Contraeña actualizada")]
    [SwaggerResponse(statusCode: 500, description: "Error al cambiar la contraseña")]
    [SwaggerResponse(statusCode: 404, description: "Usuario inexistente")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPost("{usuarioId}/contrasena")]
    public async Task<ActionResult> CambiarContrasena([FromRoute] Guid usuarioId, [FromBody] DTOCambiarContrasena datos, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        if (!datos.UsuarioId.HasValue || usuarioId != datos.UsuarioId.Value)
        {
            return BadRequest("El usuario de la actualización no corresponde con el query");
        }

        var respuesta = await servicioUsuarios.CambiarContrasena(datos.UsuarioId.Value, datos.Nueva, datos.Actual, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok();
    }

    [SwaggerOperation("Actualiza la contraseña del usuario en sesión.")]
    [SwaggerResponse(statusCode: 200, description: "Contraeña actualizada")]
    [SwaggerResponse(statusCode: 500, description: "Error al cambiar la contraseña")]
    [SwaggerResponse(statusCode: 404, description: "Usuario inexistente")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPost("mi/contrasena")]
    public async Task<ActionResult> CambiarMiContrasena([FromBody] DTOCambiarContrasena datos)
    {
        if (string.IsNullOrEmpty(datos.Actual))
        {
            return BadRequest("Debe proporcionar la contraseña actual");
        }

        var respuesta = await servicioUsuarios.CambiarContrasena(UsuarioGuid!.Value, datos.Nueva, datos.Actual, null);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok();
    }

    [SwaggerOperation("Elimina un usuario")]
    [SwaggerResponse(statusCode: 200, description: "Usuario actualizado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al actualizar usuario")]
    [SwaggerResponse(statusCode: 404, description: "Usuario inexistente")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpDelete("{usuarioId}")]
    public async Task<ActionResult> Eliminar([FromRoute] Guid usuarioId, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        var respuesta = await servicioUsuarios.Eliminar(usuarioId, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok();
    }

    [SwaggerOperation("Actualiza estado de un usuario")]
    [SwaggerResponse(statusCode: 200, description: "Usuario actualizado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al actualizar usuario")]
    [SwaggerResponse(statusCode: 404, description: "Usuario inexistente")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPut("{usuarioId}/estado/{estado}")]
    public async Task<ActionResult> EstableceEstado([FromRoute] Guid usuarioId, [FromRoute] EstadoCuenta estado, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        var respuesta = await servicioUsuarios.EstableceEstado(usuarioId, estado, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok();
    }

    [SwaggerOperation("Actualiza roles de un usuario")]
    [SwaggerResponse(statusCode: 200, description: "Usuario actualizado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al actualizar usuario")]
    [SwaggerResponse(statusCode: 404, description: "Usuario inexistente")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpPut("{usuarioId}/roles")]
    public async Task<ActionResult> EstablecerRoles([FromRoute] Guid usuarioId, [FromBody] List<string> roles, [FromQuery(Name = "eliminar")] bool eliminarAnteriores = false, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        var respuesta = await servicioUsuarios.EstablecerRoles(usuarioId, roles, eliminarAnteriores, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok();
    }

    [SwaggerOperation("Obtiene los roles de un usuario")]
    [SwaggerResponse(statusCode: 200, description: "Usuario actualizado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al actualizar usuario")]
    [SwaggerResponse(statusCode: 404, description: "Usuario inexistente")]
    [SwaggerResponse(statusCode: 400, description: "Datos no válidos")]
    [SwaggerResponse(statusCode: 403, description: "No cuenta con el provilegio")]
    [SwaggerResponse(statusCode: 401, description: "Usuario no autenticado")]
    [HttpGet("{usuarioId}/roles")]
    public async Task<ActionResult<List<string>>> ObtieneRoles([FromRoute] Guid usuarioId, [FromQuery(Name = "cfid")] Guid? cuentaFiscalId = null)
    {
        var respuesta = await servicioUsuarios.ObtieneRoles(usuarioId, cuentaFiscalId);

        if (!respuesta.Ok)
        {
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        }

        return Ok(respuesta.Payload);
    }

    [SwaggerOperation("Obtiene el perfil de usuaurio en sesión.")]
    [SwaggerResponse(200, "Perfil encontrado", typeof(PerfilUsuario))]
    [SwaggerResponse(404, "Perfil no encontrado")]
    [SwaggerResponse(403, "El usuario no tiene roles asignados")]
    [SwaggerResponse(500, "Error interno del servidor")]
    [HttpGet("perfil/mi")]
    public async Task<ActionResult<PerfilUsuario>> ObtienPerfilUsuarioEnsesion()
    {
        var respuesta = await servicioUsuarios.ObtienePerfil(UsuarioGuid!.Value);

        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);

        return Ok(respuesta.Payload);
    }

    [SwaggerOperation("Crea un token loginless")]
    [SwaggerResponse(200, "Token creado satisfactoriamente", typeof(RespuestaPayload<ResultadoTokenLoginLess>))]
    [SwaggerResponse(400, "Datos no válidos")]
    [SwaggerResponse(500, "Error interno del servidor")]
    [HttpPost("tokenloginless")]
    public async Task<ActionResult> CrearTokenLoginLess([FromBody] SolictudTokenLoginless solictudToken, [FromQuery(Name = "cfid")] Guid? CuentaFiscalId = null)
    {
        var respuesta = await servicioUsuarios.CrearTokenLoginLess(solictudToken, UsuarioGuid!.Value, CuentaFiscalId);

        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);

        return Ok();
    }

    [SwaggerOperation("Elimina un token loginless")]
    [SwaggerResponse(200, "Token eliminado satisfactoriamente")]
    [SwaggerResponse(404, "Token no encontrado")]
    [SwaggerResponse(500, "Error interno del servidor")]
    [HttpDelete("tokenloginless/{tokenId:long}")]
    public async Task<ActionResult> EliminaTokenLoginLess(
    [FromRoute] long tokenId,
    [FromQuery(Name = "cfid")] Guid? CuentaFiscalId = null)
    {
        var respuesta = await servicioUsuarios.EliminaTokenLoginLess(tokenId, CuentaFiscalId);

        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);

        return Ok();
    }

    [SwaggerOperation("Obtiene un token de vinculación")]
    [SwaggerResponse(200, "Resultado de validación", typeof(RespuestaTokenVinculacion))]
    [SwaggerResponse(500, "Error interno del servidor")]
    [AllowAnonymous]
    [HttpGet("tokenvinculacion/{dispositivoId}")]
    public async Task<ActionResult<RespuestaTokenVinculacion>> TokenVinculacion([FromRoute] string dispositivoId)
    {
        var respuesta = await servicioUsuarios.ObtieneTokenVinculacion(dispositivoId);

        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);

        return Ok(respuesta.Payload);
    }


    [SwaggerOperation("Valida un token de vinculación para un dispositivo")]
    [SwaggerResponse(200, "Resultado de validación", typeof(RespuestaTokenVinculacion))]
    [SwaggerResponse(500, "Error interno del servidor")]
    [AllowAnonymous]
    [HttpGet("tokenvinculacion/{dispositivoId}/{token}")]
    public async Task<ActionResult> ValidaActivacionToken([FromRoute] string dispositivoId, [FromRoute] string token)
    {
        var respuesta = await servicioUsuarios.VerificarActivacion(dispositivoId, token);

        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);

        return Ok();
    }

    [SwaggerOperation("Obtiene un token loginless por dispositivoId")]
    [SwaggerResponse(200, "Token encontrado", typeof(ResultadoTokenLoginLess))]
    [SwaggerResponse(404, "Token no encontrado")]
    [SwaggerResponse(500, "Error interno del servidor")]
    [AllowAnonymous]
    [HttpGet("tokenloginless/{dispositivoId}")]
    public async Task<ActionResult<ResultadoTokenLoginLess>> ObtieneTokenLoginLess([FromRoute] string dispositivoId)
    {
        var respuesta = await servicioUsuarios.ObtieneTokenLoginLess(dispositivoId);

        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);

        return Ok(respuesta.Payload);
    }

    [SwaggerOperation("Obtiene los nombres o email de los usuaios desde su id")]
    [SwaggerResponse(200, "Lista generada", typeof(List<ParIdTexto>))]
    [SwaggerResponse(500, "Error interno del servidor")]
    [HttpPost("ids")]
    public async Task<ActionResult<List<ParIdTexto>>> ObtieneNombresDeIds([FromBody] List<string> ids)
    {
        var respuesta = await servicioUsuarios.ObtieneUsuariosDeIds(ids);

        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);

        return Ok(respuesta.Payload);
    }
    [SwaggerOperation("Recupera contrasena de un usuario")]
    [SwaggerResponse(200, "DTO recupera contrasena")]
    [SwaggerResponse(404, "No se encontró el usuario")]
    [SwaggerResponse(500, "Error intero del servidor")]
    [HttpPost("contrasena/recuperar", Name = "RecuperaContrasena")]
    [AllowAnonymous]
    public async Task<ActionResult<DTORecuperaContrasena>> RecuperaContrasena([FromQuery] string email)
    {
        var respuesta = await servicioUsuarios.RecuperaContrasenaEmail(email);
        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        return Ok(respuesta.Payload);
    }

    [SwaggerOperation("Esteblece nueva contraseña")]
    [SwaggerResponse(200, "Contraseña actualizada")]
    [SwaggerResponse(404, "Usuario no encontrado")]
    [SwaggerResponse(500, "Error interno del servidor")]
    [HttpPost("contrasena/token", Name = "EstableceContrasena")]
    [AllowAnonymous]
    public async Task<IActionResult> EstableceContrasenaToken([FromBody] ActualizaContrasena actualizaContrasena)
    {
        var respuesta = await servicioUsuarios.EstablaceContrasenaToken(actualizaContrasena);
        if (!respuesta.Ok)
            return ActionFromCode(respuesta.HttpCode, respuesta.Error);
        return Ok();
    }

}
