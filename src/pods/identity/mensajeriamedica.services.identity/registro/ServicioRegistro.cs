using System.Net;
using comunes.proxies.proxygenerico;
using comunes.respuestas;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.usuarios;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.identity.registro;

/// <summary>
/// Servicio de procesamiento del registro de usuarios.
/// </summary>
/// <param name="logger">Loggr.</param>
/// <param name="userManager">Servicio de usuarios identidad.</param>
/// <param name="proxyGenerico">Proxy interservicios genérico.</param>
public class ServicioRegistro(ILogger<ServicioRegistro> logger, UserManager<ApplicationUser> userManager,IProxyGenericoInterservicio proxyGenerico, DbContextIdentity context) : IServicioRegistro
{
    public async Task<Respuesta> Registro(RegisterViewModel registro)
    {
        logger.LogDebug("ServicioRegistro-Registro {Email}", registro.Email);

        Respuesta respuesta = new ();
        ApplicationUser? user = await userManager.FindByEmailAsync(registro.Email);

        if (user != null)
        {
            if (user.EmailConfirmed)
            {
                logger.LogDebug("ServicioRegistro-Registro el {Email} usuario ya existe.", registro.Email);

                respuesta.Error = new ()
                {
                    HttpCode = HttpStatusCode.Conflict,
                    Codigo = CodigosError.IDENTITY_REGISTRO_EMAIL_DUPLICADO,
                    Mensaje = "Email duplicado"
                };
                respuesta.HttpCode = HttpStatusCode.Conflict;
                return respuesta;
            }
            else
            {
                logger.LogDebug("ServicioRegistro-Registro el {Email} usuario ya existe pero no ha sido confirmado.", registro.Email);
            }
        }
        else
        {
            user = new ApplicationUser() { UserName = registro.Email, Email = registro.Email };
            var result = await userManager.CreateAsync(user, registro.Password);
            if (!result.Succeeded)
            {
                respuesta.Error = new ()
                {
                    HttpCode = HttpStatusCode.InternalServerError,
                    Codigo = CodigosError.IDENTITY_ERROR_REGISTRO,
                    Mensaje = string.Empty
                };

                if (result.Errors != null && result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                    {
                        respuesta.Error.Mensaje += $"{error.Code} {error.Description}";
                    }
                }

                logger.LogError("ServicioRegistro-Registro error {Error} al crear el usuario {Email}.", respuesta.Error, registro.Email);

                respuesta.HttpCode = HttpStatusCode.InternalServerError;
                return respuesta;
            }
        }

        if (!string.IsNullOrWhiteSpace(registro.DispositivoId))
        {
            var dispositivoUsuario = new DispositivoUsuario
            {
                UsuarioId = user.Id,
                DispositivoId = registro.DispositivoId
            };

            context.DispositivosUsuario.Add(dispositivoUsuario);
            await context.SaveChangesAsync();
        }

        logger.LogDebug("ServicioRegistro-Registro enviando correo de confirmación a {Email}.", registro.Email);
        //var respuestaEmail = await proxyGenerico.JsonPost("comunicaciones", "/email/confirmacion", "Envio email confirmacion", new DatosConfirmacion() { Email = user!.Email!, UsuarioId = user.Id });

        //if (!respuestaEmail.Ok)
        //{
        //    logger.LogError("ServicioRegistro-Registro error al enviar confirmación a {Email} {Error}.", registro.Email, $"{respuestaEmail.HttpCode} {respuestaEmail.Error?.Codigo} {respuestaEmail.Error?.Mensaje}");

        //    respuesta.Error = respuestaEmail.Error;
        //    respuesta.Error!.Codigo = CodigosError.IDENTITY_ERROR_ENVIO_CONFIRMACION;
        //    respuesta.HttpCode = HttpStatusCode.InternalServerError;
        //    return respuesta;
        //}
        //else
        //{
        //    respuesta.Ok = true;
        //}

        respuesta.Ok = true;

        return respuesta;
    }

    public async Task<RespuestaPayload<DatosRegistro>> GetRegistroConfirmar(string id)
    {
        logger.LogDebug("ServicioRegistro-GetRegistroConfirmar solicitud confirmacion para {Id}.", id);
        RespuestaPayload<DatosRegistro> respuestaPayload = new ();
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            logger.LogDebug("ServicioRegistro-GetRegistroConfirmar solicitud no localizada para {Id}.", id);
            respuestaPayload.Error = new ()
            {
                HttpCode = HttpStatusCode.NotFound,
                Codigo = CodigosError.IDENTITY_REGISTRO_SOLICITUD_NO_ENCONTRADA,
                Mensaje = "Solictud registro no encontrada"
            };
            respuestaPayload.HttpCode = HttpStatusCode.NotFound;
            return respuestaPayload;
        }

        logger.LogDebug("ServicioRegistro-GetRegistroConfirmar solicitud localizada para {Id} on email {Email}.", id, user.Email);
        respuestaPayload.Ok = true;
        respuestaPayload.Payload = new DatosRegistro() { Email = user!.Email! };
        return respuestaPayload;
    }

    public async Task<Respuesta> PostRegistroConfirmar(string id)
    {
        logger.LogDebug("ServicioRegistro-PostRegistroConfirmar solicitud confirmacion para {Id}.", id);
        Respuesta respuesta = new ();
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            logger.LogDebug("ServicioRegistro-PostRegistroConfirmar solicitud no localizada para {Id}.", id);
            respuesta.Error = new ()
            {
                HttpCode = HttpStatusCode.NotFound,
                Codigo = CodigosError.IDENTITY_REGISTRO_SOLICITUD_NO_ENCONTRADA,
                Mensaje = "Email no encontrado"
            };
            respuesta.HttpCode = HttpStatusCode.NotFound;
            return respuesta;
        }

        if (user.EmailConfirmed)
        {
            logger.LogDebug("ServicioRegistro-PostRegistroConfirmar el usuario ya ha sido verificado {Id}.", id);
            respuesta.Error = new ()
            {
                HttpCode = HttpStatusCode.NotFound,
                Codigo = CodigosError.IDENTITY_REGISTRO_SOLICITUD_NO_ENCONTRADA,
                Mensaje = "Usuario verificado con anterioridad"
            };
            respuesta.HttpCode = HttpStatusCode.NotFound;
            return respuesta;
        }

        logger.LogDebug("ServicioRegistro-PostRegistroConfirmar actualizando datos para {Email}.", user.Email);
        user.EmailConfirmed = true;
        user.FechaActivacion = DateTime.UtcNow;
        var respuestaUpdate = await userManager.UpdateAsync(user);

        if (!respuestaUpdate.Succeeded)
        {
            respuesta.Error = new ()
            {
                HttpCode = HttpStatusCode.InternalServerError,
                Codigo = CodigosError.IDENTITY_ERROR_REGISTRO,
                Mensaje = string.Empty
            };

            if (respuestaUpdate.Errors != null && respuestaUpdate.Errors.Any())
            {
                foreach (var error in respuestaUpdate.Errors)
                {
                    respuesta.Error.Mensaje += $"{error.Code} {error.Description}";
                }
            }

            logger.LogError("ServicioRegistro-PostRegistroConfirmar error {Error} al confirmar el usuario {Email}.", respuesta.Error, user.Email);

            respuesta.HttpCode = HttpStatusCode.InternalServerError;
            return respuesta;
        }

        logger.LogDebug("ServicioRegistro-PostRegistroConfirmar creación de la cuenta de CRM para {Email}.", user.Email);
        var result = await proxyGenerico.JsonPost("crm", $"/crm/cuentaprimaria/{id}", $"Crear cuenta primaria CRM usuario {id}", null);

        if (!result.Ok)
        {
            respuesta.Error = new ()
            {
                HttpCode = HttpStatusCode.InternalServerError,
                Codigo = CodigosError.IDENTITY_ERROR_CONFIRMACION_CUENTA,
                Mensaje = "Erro en la creación de la cuenta en el CRM"
            };
            respuesta.HttpCode = HttpStatusCode.NotFound;
            logger.LogError("ServicioRegistro-PostRegistroConfirmar error {Error} al crear cuenta de CRM para el usuario {Email}.", $"{result.HttpCode} {result.Error?.Codigo} {result.Error?.Mensaje}", user.Email);
            return respuesta;
        }

        respuesta.Ok = true;
        return respuesta;
    }
}