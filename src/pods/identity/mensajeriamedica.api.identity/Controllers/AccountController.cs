using comunes;
using comunes.respuestas;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.services.identity.registro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenIddict.Validation.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;

namespace mensajeriamedica.api.identity.Controllers;
[ApiController]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
#pragma warning disable S6934 // A Route attribute should be added to the controller when a route template is specified at the action level
public class AccountController(ILogger<AccountController> logger, IServicioRegistro servicioRegistro, UserManager<ApplicationUser> userManager, IConfiguration configuration) : ControllerUsoInterno(logger)
#pragma warning restore S6934 // A Route attribute should be added to the controller when a route template is specified at the action level
{
    [SwaggerOperation("Registra un nuevo usuario")]
    [SwaggerResponse(statusCode: 200, description: "Usuario Registrado satisfactoriamente")]
    [SwaggerResponse(statusCode: 500, description: "Error al crear registro")]
    [SwaggerResponse(statusCode: 409, description: "El usuario ya existe")]
    [HttpPost("usuario/registro")]
    [AllowAnonymous]
    public async Task<ActionResult> Registro([FromBody] RegisterViewModel registro)
    {

        if(registro.Code != configuration.GetValue<string>("CodigoRegistro"))
        {
            return BadRequest("Código de registro erróneo");
        }

        logger.LogDebug("AccountController-Registro");
        var result = await servicioRegistro.Registro(registro);

        if (result.Ok)
        {
            logger.LogDebug("AccountController-Registro operacion exitosa");
            return Ok();
        }

        return ActionFromCode(result.HttpCode, result.Error!.Mensaje);
    }

    [SwaggerOperation("Valida que exista un registro de confirmación")]
    [SwaggerResponse(statusCode: 200, type: typeof(DatosRegistro), description: "Registro existente")]
    [SwaggerResponse(statusCode: 404, description: "Registro no encontrado")]
    [SwaggerResponse(statusCode: 500, description: "Error al localizar registro")]
    [HttpGet("usuario/registro/confirmar/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RespuestaPayload<DatosRegistro>>> GetRegistroConfirmar(string id)
    {
        var result = await servicioRegistro.GetRegistroConfirmar(id);

        if (result.Ok)
        {
            logger.LogDebug("AccountController-GetRegistroConfirmar operacion exitosa");
            return Ok(result.Payload);
        }

        return ActionFromCode(result.HttpCode, result.Error!.Mensaje);
    }

    [SwaggerOperation("Confirma el registro de un usuario")]
    [SwaggerResponse(statusCode: 200, description: "Confirmación exitosa")]
    [SwaggerResponse(statusCode: 404, description: "Confirmación no encontrada")]
    [SwaggerResponse(statusCode: 500, description: "Error al confirmar registro")]
    [HttpPost("usuario/registro/confirmar/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> PostRegistroConfirmar(string id)
    {
        logger.LogDebug("AccountController-PostRegistroConfirmar");
        var result = await servicioRegistro.PostRegistroConfirmar(id);

        if (result.Ok)
        {
            logger.LogDebug("AccountController-PostRegistroConfirmar operacion exitosa");
            return Ok();
        }

        return ActionFromCode(result.HttpCode, result.Error!.Mensaje);
    }

    [Authorize(Roles = "AdminUsuarios,AdminSistema,interproceso")]
    [SwaggerOperation("Obtiene los roles de un usuario")]
    [SwaggerResponse(statusCode: 200, type: typeof(List<string>), description: "Roles asociados al usuario")]
    [SwaggerResponse(statusCode: 404, description: "Registro no encontrado")]
    [SwaggerResponse(statusCode: 500, description: "Error al localizar registro")]
    [HttpGet("usuario/{id}/roles")]
    public async Task<ActionResult<RespuestaPayload<List<string>>>> ObtieneRolesUsuario(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);

        return Ok(roles.ToList());
    }

    [SwaggerOperation("Obtiene lista de los usuarios")]
    [SwaggerResponse(statusCode: 200, type: typeof(List<DtoUsuario>), description: "Lista de Usuarios")]
    [SwaggerResponse(statusCode: 404, description: "Registro no encontrado")]
    [SwaggerResponse(statusCode: 500, description: "Error al localizar registro")]
    [HttpGet("usuarios/lista")]
    public async Task<ActionResult<List<DtoUsuario>>> ObtieneListaUsuario()
    {
        var usuarios = await userManager.Users.Select(u => new DtoUsuario{Id = Guid.Parse(u.Id) ,Email = u.Email}).ToListAsync();

        return Ok(usuarios);
    }
}
