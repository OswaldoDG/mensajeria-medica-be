using System.Data;
using System.Net;
using comunes.auth;
using comunes.busqueda;
using comunes.extensiones;
using comunes.proxies.proxygenerico;
using comunes.respuestas;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.model.identity.usuarios;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CuentaUsuario = mensajeriamedica.model.identity.usuarios.CuentaUsuario;
using TipoCuenta = mensajeriamedica.model.identity.registro.TipoCuenta;

namespace mensajeriamedica.services.identity.usuarios;

/// <summary>
/// Servicio de usuarios de la aplicacion.
/// </summary>
public class ServicioUsuarios(ILogger<ServicioUsuarios> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
    DbContextIdentity db, IDistributedCache cache, IProxyGenericoInterservicio proxy, IDistributedCache _cache) : IServicioUsuarios
{

    public async Task<RespuestaPayload<PerfilUsuario>> ObtienePerfil(Guid usuarioId)
    {
        RespuestaPayload<PerfilUsuario> respuesta = new ();
        try
        {
            logger.LogDebug("ServicioUsuarios-ObtienePerfil {UsuarioId}", usuarioId);
            var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
            if (usuario != null)
            {
                var roles = await userManager.GetRolesAsync(usuario);
                if (roles != null && roles.Count > 0)
                {
                    respuesta.Payload = new ()
                    {
                        CuentaFiscalId = usuario.CuentaFiscalId,
                        DisplayName = usuario.Nombre,
                        EsInterno = usuario.TipoCuenta == TipoCuenta.Empleado,
                        Iniciales = usuario.Nombre ?? usuario.UserName ?? "",
                        Roles = roles != null ? [.. roles] : []
                    };
                }
                else
                {
                    logger.LogDebug("ServicioUsuarios-ObtienePerfil usuario sin rol asignado {UsuarioId}.", usuarioId);
                    respuesta.Error = ErroresUsuario.UsuarioSinRol();
                }
            }
            else
            {
                logger.LogDebug("ServicioUsuarios-ObtienePerfil usuario inexistente {UsuarioId}.", usuarioId);
                respuesta.Error = ErroresUsuario.UsuarioInexistente();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-Crear ObtienePerfil al obtener el perfil {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<RespuestaPayload<CuentaUsuario>> CrearUsuarioCaptura(CreaUsuarioCaptura datos, Guid usuarioCreadorId, Guid cuentaFiscalId)
    {
        RespuestaPayload<CuentaUsuario> resultado = new ();
        string claveCacheCF = $"cf-{cuentaFiscalId}";
        comunes.servicios.crm.CuentaFiscal? cuentaFiscal = _cache.LeeSerializado<comunes.servicios.crm.CuentaFiscal>(claveCacheCF);
        if (cuentaFiscal == null)
        {
            var respuestaProxy = await proxy.JsonRespuestaSerializada("crm", $"crm/cuentafiscal/{cuentaFiscalId}?direcciones=false&textoClaves=false", "Obtener cuenta fiscal", VerboHttp.GET, null);
            if (respuestaProxy.Ok)
            {
                cuentaFiscal = JsonConvert.DeserializeObject<comunes.servicios.crm.CuentaFiscal>(respuestaProxy!.Payload!);
                _cache.AlmacenaSerializadoSliding(claveCacheCF, cuentaFiscal!, 5);
            }
        }

        if (cuentaFiscal != null)
        {
#pragma warning disable S6966 // Awaitable method should be used
            var emails = db.Users.Where(u => u.CuentaFiscalId == datos.CuentaFiscalId && u.Email != null && u.Email.StartsWith("captura")).Select(u => u.Email).ToList();
#pragma warning restore S6966 // Awaitable method should be used

#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            emails = [.. emails.Where(e => e.EndsWith("contabee.mx"))];
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

            int longitud = cuentaFiscal.Tipo == comunes.servicios.crm.TipoPersonaFiscal.Fisica ? 4 : 3; // RFC fisica tiene 4 caracteres, moral tiene 3
            int id = 1;
            string email = $"captura-{cuentaFiscal.RFC[..longitud]}-{id.ToString().PadLeft(3, '0')}@contabee.mx";
            while (emails.Contains(email))
            {
                id++;
                email = $"captura-{cuentaFiscal.RFC[..longitud]}-{id.ToString().PadLeft(3, '0')}@contabee.mx";
            }

            CreaUsuario usuario = new CreaUsuario
            {
                CuentaFiscalId = datos.CuentaFiscalId,
                Email = email,
                Nombre = datos.Nombre,
                Password = datos.Password,
                Telefono = datos.Telefono,
                TipoCuenta = TipoCuenta.EmpleadoCliente
            };

            resultado = await Crear(usuario, usuarioCreadorId, cuentaFiscalId);
        }

        return resultado;
    }

    public async Task<RespuestaPayload<CuentaUsuario>> Crear(CreaUsuario datos, Guid usuarioCreadorId, Guid? cuentaFiscalId = null)
    {
        logger.LogDebug("ServicioUsuarios-Crear {Tipo} {Creador} {CuentaFiscalId}", datos.TipoCuenta, usuarioCreadorId, cuentaFiscalId);

        RespuestaPayload<CuentaUsuario> respuesta = new ();
        try
        {
            var configuraUsuario = datos.CreaUsuarioApp(usuarioCreadorId, cuentaFiscalId);

            if (configuraUsuario.Payload != null)
            {
                if (datos.TipoCuenta != TipoCuenta.LoginLessCliente && !string.IsNullOrEmpty(datos.Email) && await userManager.FindByEmailAsync(datos.Email!) != null)
                {
                    logger.LogDebug("ServicioUsuarios-Crear el usuario ya existe.");
                    respuesta.Error = ErroresUsuario.UsuarioExistente();
                }
                else
                {
                    logger.LogDebug("ServicioUsuarios-Crear iniciando proceso de creación.");

                    IdentityResult usuarioCreado;
                    if (datos.TipoCuenta != TipoCuenta.LoginLessCliente)
                    {
                        configuraUsuario.Payload.UserName = datos.Email;
                        usuarioCreado = await userManager.CreateAsync(configuraUsuario.Payload, datos.Password!);
                    }
                    else
                    {
                        // El usuario es loginless
                        usuarioCreado = await userManager.CreateAsync(configuraUsuario.Payload);
                    }

                    if (usuarioCreado.Succeeded)
                    {
                        logger.LogDebug("ServicioUsuarios-Crear registro adicionado al repositorio.");

                        respuesta.Ok = true;

                        if (respuesta.Ok)
                        {
                            logger.LogDebug("ServicioUsuarios-Crear creación exitosa.");
                            respuesta.Payload = configuraUsuario.Payload.ToCuentaUsuario();
                        }
                    }
                    else
                    {
                        string error = string.Join(',', [.. usuarioCreado.Errors.Select(e => $"{e.Code} {e.Description}")]);
                        logger.LogError("ServicioUsuarios-Crear error al crear usuario {Error}.", error);
                        respuesta.Error = ErroresUsuario.CracionDesconocido(error);
                    }
                }
            }
            else
            {
                logger.LogDebug("ServicioUsuarios-Crear errores de validación encontrados");
                respuesta.Error = configuraUsuario.Error;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-Crear Error al realizar la búsqueda {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<Respuesta> Actualizar(Guid usuarioId, ActualizaUsuario datos, Guid? cuentaFiscalId = null)
    {
        Respuesta respuesta = new ();
        try
        {
            logger.LogDebug("ServicioUsuarios-Actualizar actualizar {UsuarioId} {CuentaFiscalId}", usuarioId, cuentaFiscalId);
            var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
            if (usuario == null)
            {
                respuesta.Error = ErroresUsuario.UsuarioInexistente();
            }
            else
            {
                if (cuentaFiscalId.HasValue && usuario.CuentaFiscalId != cuentaFiscalId)
                {
                    respuesta.Error = ErroresUsuario.CuentafiscalNoValida();
                }
                else
                {
                    if (!string.IsNullOrEmpty(usuario.Email) && string.IsNullOrEmpty(datos.Email))
                    {
                        respuesta.Error = ErroresUsuario.CondicionDatos("no es posible asignar nulo a un email existente");
                    }
                    else
                    {
                        bool actualizarEmail = !string.IsNullOrEmpty(datos.Email) && datos.Email != usuario.Email;

                        usuario.PhoneNumber = datos.Telefono;
                        usuario.Nombre = datos.Nombre;
                        usuario.Email = datos.Email;
                        var resultadoUpdate = await userManager.UpdateAsync(usuario);
                        if (resultadoUpdate.Succeeded)
                        {
                            respuesta.Ok = true;
                        }
                        else
                        {
                            string error = string.Join(',', [.. resultadoUpdate.Errors.Select(e => $"{e.Code} {e.Description}")]);
                            logger.LogError("ServicioUsuarios-Actualizar error al actualizar datos del usuario {Error}.", error);
                            respuesta.Error = ErroresUsuario.ActualizarDesconocido(error);
                        }
                        if (actualizarEmail)
                        {
                            await userManager.UpdateNormalizedEmailAsync(usuario);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-Actualizar Error al realizar la actualizacion {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public Task<RespuestaPayload<ResultadoPaginado<CuentaUsuario>>> Buscar(Busqueda busqueda, Guid? cuentaFiscalId = null)
    {
        RespuestaPayload<ResultadoPaginado<CuentaUsuario>> respuesta = new ();
        logger.LogDebug("ServicioUsuarios-Buscar búsqueda {Busqueda} {CuentaFiscalId}", busqueda, cuentaFiscalId);
        try
        {
            busqueda = busqueda.EliminaFiltrosUsuarioNoValidos(cuentaFiscalId);
            respuesta.Error = busqueda.ValidaContextoBusqueda(cuentaFiscalId);
            if (respuesta.Error == null)
            {
                respuesta.Payload = Extensiones.BusquedaConRol(busqueda, db);
                respuesta.Ok = true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-Buscar Error al realizar la búsqueda {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return Task.FromResult(respuesta);
    }

    public async Task<Respuesta> CambiarContrasena(Guid usuarioId, string contrasenaNueva, string? contrasenaAnterior = null, Guid? cuentaFiscalId = null)
    {
        Respuesta respuesta = new ();
        try
        {
            logger.LogDebug("ServicioUsuarios-Buscar CambiarContrasena {UsuarioId} {CuentaFiscalID}", usuarioId, cuentaFiscalId);
            var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
            if (usuario == null)
            {
                respuesta.Error = ErroresUsuario.UsuarioInexistente();
            }
            else
            {
                logger.LogDebug("ServicioUsuarios-CambiarContrasena validando contexto");

                if (!string.IsNullOrEmpty(contrasenaAnterior) && !(await userManager.CheckPasswordAsync(usuario, contrasenaAnterior)))
                {
                    respuesta.Error = ErroresUsuario.ContrasenaActualNoValida();
                }

                if (cuentaFiscalId.HasValue && usuario.CuentaFiscalId != cuentaFiscalId)
                {
                    respuesta.Error = ErroresUsuario.CuentafiscalNoValida();
                }
            }

            if (respuesta.Error == null)
            {
                await userManager.RemovePasswordAsync(usuario!);
                var setPassword = await userManager.AddPasswordAsync(usuario!, contrasenaNueva);
                if (setPassword.Succeeded)
                {
                    respuesta.Ok = true;
                }
                else
                {
                    string error = string.Join(',', [.. setPassword.Errors.Select(e => $"{e.Code} {e.Description}")]);
                    logger.LogError("ServicioUsuarios-CambiarContrasena error al cambiar la contraseña del usuario {Error}.", error);
                    respuesta.Error = ErroresUsuario.CambioPasswordDesconocido(error);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-CambiarContrasena Error al realizar la búsqueda {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<Respuesta> Eliminar(Guid usuarioId, Guid? cuentaFiscalId = null)
    {
        Respuesta respuesta = new ();
        logger.LogDebug("ServicioUsuarios-Eliminar eliminando {UsuarioId} {CuentaFiscalId}", usuarioId, cuentaFiscalId);
        try
        {
            var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
            if (usuario == null)
            {
                respuesta.Error = ErroresUsuario.UsuarioInexistente();
            }

            if (cuentaFiscalId.HasValue && usuario!.CuentaFiscalId != cuentaFiscalId)
            {
                respuesta.Error = ErroresUsuario.CuentafiscalNoValida();
            }

            if (respuesta.Error == null)
            {
                var respuestaEliminar = await userManager.DeleteAsync(usuario!);
                if (respuestaEliminar.Succeeded)
                {
                    respuesta.Ok = true;
                }
                else
                {
                    string error = string.Join(',', [.. respuestaEliminar.Errors.Select(e => $"{e.Code} {e.Description}")]);
                    logger.LogError("ServicioUsuarios-Eliminar error al eliminar usuario {Error}.", error);
                    respuesta.Error = ErroresUsuario.EliminarDesconocido(error);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-Eliminar Error eliminar usuario {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<Respuesta> EstableceEstado(Guid usuarioId, model.identity.registro.EstadoCuenta estado, Guid? cuentaFiscalId = null)
    {
        Respuesta respuesta = new ();
        try
        {
            logger.LogDebug("ServicioUsuarios-EstableceEstado nuevo estado {Estado} para {UsuarioId} {CuentaFiscalId}", estado, usuarioId, cuentaFiscalId);
            var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
            if (usuario == null)
            {
                respuesta.Error = ErroresUsuario.UsuarioInexistente();
            }
            else
            {
                if (cuentaFiscalId.HasValue && usuario.CuentaFiscalId != cuentaFiscalId)
                {
                    respuesta.Error = ErroresUsuario.CuentafiscalNoValida();
                }
                else
                {
                    usuario.Estado = estado;
                    var resultadoUpdate = await userManager.UpdateAsync(usuario);
                    if (resultadoUpdate.Succeeded)
                    {
                        respuesta.Ok = true;
                    }
                    else
                    {
                        string error = string.Join(',', [.. resultadoUpdate.Errors.Select(e => $"{e.Code} {e.Description}")]);
                        logger.LogError("ServicioUsuarios-EstableceEstado error al actualizar datos del usuario {Error}.", error);
                        respuesta.Error = ErroresUsuario.ActualizarDesconocido(error);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-Actualizar Error al realizar la actualizacion {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<Respuesta> EstablecerRoles(Guid usuarioId, List<string> roles, bool eliminarAnteriores = false, Guid? cuentaFiscalId = null)
    {
        Respuesta respuesta = new ();
        try
        {
            logger.LogDebug("ServicioUsuarios-EstablecerRoles roles para {UsuarioId} {CuentaFiscalId}", usuarioId, cuentaFiscalId);

            var rolesSistema = ObtieneRolesSistema();
            if (!roles.RolesValidos(rolesSistema ?? []))
            {
                respuesta.Error = ErroresUsuario.RolNoValido();
            }
            else
            {
                var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
                if (usuario == null)
                {
                    respuesta.Error = ErroresUsuario.UsuarioInexistente();
                }
                else
                {
                    if (cuentaFiscalId.HasValue && usuario.CuentaFiscalId != cuentaFiscalId)
                    {
                        respuesta.Error = ErroresUsuario.CuentafiscalNoValida();
                    }
                    else
                    {
                        if (usuario.CuentaFiscalId.HasValue)
                        {
                            if (cuentaFiscalId.HasValue && (usuario.CuentaFiscalId.Value != cuentaFiscalId.Value))
                            {
                                respuesta.Error = ErroresUsuario.CuentafiscalNoValida();
                            }
                            else
                            {
                                bool esInsert = true;
                                RolCuentaFiscal? rol = await db.RolesCuentaFiscal.FirstOrDefaultAsync(r => r.CuentaFiscalId == usuario.CuentaFiscalId.Value && r.UsuarioId == usuarioId);
                                if (rol == null)
                                {
                                    esInsert = true;
                                    rol = new () { CuentaFiscalId = usuario.CuentaFiscalId.Value, UsuarioId = usuarioId, Roles = string.Empty };
                                }

                                rol.Roles = ExtensionesUsuario.StringRolesActualizados(string.IsNullOrEmpty(rol.Roles) ? [] : [.. rol.Roles.Split(',')], roles, eliminarAnteriores);

                                if (esInsert)
                                {
                                    await db.RolesCuentaFiscal.AddAsync(rol);
                                }
                                else
                                {
                                    db.RolesCuentaFiscal.Update(rol);
                                }

                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            var rolesActuales = await userManager.GetRolesAsync(usuario);
                            if (eliminarAnteriores)
                            {
                                await userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                                rolesActuales = roles;
                            }
                            else
                            {
                                foreach (var rol in roles)
                                {
                                    if (!rolesActuales.Any(x => x.Equals(rol, StringComparison.CurrentCultureIgnoreCase)))
                                    {
                                        rolesActuales.Add(rol);
                                    }
                                }
                            }

                            var resultadoUpdate = await userManager.AddToRolesAsync(usuario, rolesActuales);
                            if (resultadoUpdate.Succeeded)
                            {
                                respuesta.Ok = true;
                            }
                            else
                            {
                                string error = string.Join(',', [.. resultadoUpdate.Errors.Select(e => $"{e.Code} {e.Description}")]);
                                logger.LogError("ServicioUsuarios-EstablecerRoles error al actualizar roles del usuario {Error}.", error);
                                respuesta.Error = ErroresUsuario.ActualizarRolesDesconocido(error);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-Actualizar Error al realizar la actualizacion {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<RespuestaPayload<List<string>>> ObtieneRoles(Guid usuarioId, Guid? cuentaFiscalId = null)
    {
        RespuestaPayload<List<string>> respuesta = new ();
        try
        {
            logger.LogDebug("ServicioUsuarios-ObtieneRoles para {UsuarioId} {CuentaFiscalId}", usuarioId, cuentaFiscalId);
            var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
            if (usuario == null)
            {
                respuesta.Error = ErroresUsuario.UsuarioInexistente();
            }
            else
            {
                if (cuentaFiscalId.HasValue && usuario.CuentaFiscalId != cuentaFiscalId)
                {
                    respuesta.Error = ErroresUsuario.CuentafiscalNoValida();
                }
                else
                {
                    respuesta.Payload = (await userManager.GetRolesAsync(usuario)).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-ObtieneRoles Error al obtener roles {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<Respuesta> CrearTokenLoginLess(SolictudTokenLoginless solicitudToken, Guid usuarioCreadorId, Guid? cuentaFiscalId = null)
    {
        Respuesta respuesta = new ();

        try
        {
            if (cuentaFiscalId.HasValue)
            {
                solicitudToken.CuentaFiscalId = cuentaFiscalId.Value;
            }

            logger.LogDebug("ServicioUsuarios-CrearTokenLoginLess determina la existencia de un TokenVinculacion {Token}.", solicitudToken.TokenVinculacion);

            var existeTokenVinculacion = await db.TokensVinculacion.FirstOrDefaultAsync(e => e.Token == solicitudToken.TokenVinculacion);

            if (existeTokenVinculacion is null)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "No existe ningún Token de Vinculación",
                    HttpCode = HttpStatusCode.NotFound,
                    Codigo = CodigosError.IDENTITY_SERVICIO_USUARIOS_TOKEN_VINCULACION_NO_ENCONTRADO
                };
                respuesta.HttpCode = HttpStatusCode.NotFound;
                return respuesta;
            }

            logger.LogDebug("ServicioUsuarios-CrearTokenLoginLess determina la existencia de un DispositivoUsuario con {DispositivoId}.", existeTokenVinculacion.DeviceId);

            var existeDipositivo = await db.DispositivosUsuario.FirstOrDefaultAsync(e => e.DispositivoId == existeTokenVinculacion.DeviceId);

            CuentaUsuario cuentaUsuario;

            if (existeDipositivo is null)
            {

                logger.LogDebug("ServicioUsuarios-CrearTokenLoginLess crea un nuevo usuario LoginLess");

                var datosUsuario = new CreaUsuario()
                {
                    Email = solicitudToken.Email,
                    Nombre = solicitudToken.Nombre,
                    Telefono = solicitudToken.Telefono,
                    TipoCuenta = TipoCuenta.LoginLessCliente,
                    CuentaFiscalId = cuentaFiscalId
                };
                var usuarioLoginLess = await this.Crear(datosUsuario, usuarioCreadorId, cuentaFiscalId);

                if (usuarioLoginLess.Ok)
                {
                    cuentaUsuario = usuarioLoginLess.Payload!;
                    logger.LogDebug("ServicioUsuarios-CrearTokenLoginLess inserción de una nueva relación DispositivoUsuario con el {Usuario} generado", cuentaUsuario!.Email);
                    db.DispositivosUsuario.Add(new DispositivoUsuario() { UsuarioId = cuentaUsuario.Id.ToString(), DispositivoId = existeTokenVinculacion.DeviceId });
                    await db.SaveChangesAsync();
                }
                else
                {
                    respuesta.Error = ErroresUsuario.CracionDesconocido("El usuario no pudo crearse.");
                    respuesta.HttpCode = HttpStatusCode.BadRequest;
                    return respuesta;
                }
            }
            else
            {
                logger.LogDebug("ServicioUsuarios-CrearTokenLoginLess busqueda del usuario registrado {Usuario}", existeDipositivo!.UsuarioId);
                var usuarioRegistrado = await userManager.FindByIdAsync(existeDipositivo!.UsuarioId);
                cuentaUsuario = usuarioRegistrado!.ToCuentaUsuario()!;
            }

            logger.LogDebug("ServicioUsuarios-CrearTokenLoginLess Creación de TokenLoginLess");

            if (solicitudToken.Caducidad.HasValue && solicitudToken.Caducidad <= DateTime.UtcNow.AddDays(30))
            {
                respuesta.Error = ErroresUsuario.CondicionDatos("La fecha de caducidad debe estar en el futuro si el token caduca y debe ser de al menos 30 días.");
                respuesta.HttpCode = HttpStatusCode.BadRequest;
                return respuesta;
            }

            var tokenLoginLess = await db.TokensLoginless.FirstOrDefaultAsync(t => t.UsuarioId == cuentaUsuario.Id.ToString());

            if (tokenLoginLess == null)
            {
                tokenLoginLess = new TokenLoginLess
                {
                    UsuarioId = cuentaUsuario!.Id.ToString(),
                    Token = Guid.NewGuid().ToString(),
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreador = usuarioCreadorId
                };

                db.TokensLoginless.Add(tokenLoginLess);
            }
            
            logger.LogDebug("ServicioUsuarios-CrearTokenLoginLess Actualizando la propiedad Activado = true del TokenVinculación");

            existeTokenVinculacion.Activado = true;
            db.Update(existeTokenVinculacion);
            await db.SaveChangesAsync();

            respuesta.Ok = true;
            respuesta.HttpCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-CrearTokenLoginLess Error: {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<Respuesta> EliminaTokenLoginLess(long tokenId, Guid? cuentaFiscalId = null)
    {
        Respuesta respuesta = new ();
        try
        {
            var token = await db.TokensLoginless.FirstOrDefaultAsync(t => t.Id == tokenId);

            if (token == null)
            {
                respuesta.Error = ErroresUsuario.RecursoNoEncontrado("Token loginless");
                respuesta.HttpCode = HttpStatusCode.NotFound;
                return respuesta;
            }

            db.TokensLoginless.Remove(token);
            await db.SaveChangesAsync();
            respuesta.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios-EliminaTokenLoginLess Error: {Mensaje}", ex.Message);
            respuesta.Error = ex.ToError();
        }

        return respuesta;
    }

    public async Task<RespuestaPayload<ResultadoValidacionInternaToken>> ValidaTokenLoginLess(string token)
    {
        var respuesta = new RespuestaPayload<ResultadoValidacionInternaToken>();
        string clave = $"tless{token}";
        ResultadoValidacionInternaToken? validacion = cache.LeeSerializado<ResultadoValidacionInternaToken>(clave);
        if (validacion == null)
        {
            validacion = new ResultadoValidacionInternaToken();
            var existeToken = await db.TokensLoginless.FirstOrDefaultAsync(t => t.Token == token);
            if (existeToken == null)
            {
                validacion.Valido = false;
            }
            else
            {
                validacion.Valido = true;
                validacion.UsuarioId = Guid.Parse(existeToken.UsuarioId);
            }

            cache.AlmacenaSerializadoSliding(clave, validacion);
        }

        respuesta.Payload = validacion;
        respuesta.Ok = true;
        return respuesta;
    }

    public async Task<RespuestaPayload<ResultadoTokenLoginLess>> ObtieneTokenLoginLess(string dispositivoId)
    {
        var respuesta = new RespuestaPayload<ResultadoTokenLoginLess>();

        try
        {
#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
            DispositivoUsuario dispositivo = await db.DispositivosUsuario.FirstOrDefaultAsync(d => d.DispositivoId == dispositivoId.ToString());
            if (dispositivo != null)
            {
                ApplicationUser usuario = await db.Users.Where(u => u.Id == dispositivo!.UsuarioId).FirstOrDefaultAsync();

                if (usuario == null)
                {
                    respuesta.HttpCode = HttpStatusCode.NotFound;
                    respuesta.Error = ErroresUsuario.RecursoNoEncontrado("Usuario");
                    return respuesta;
                }

                TokenLoginLess tokenExistente = await db.TokensLoginless.Where(t => t.UsuarioId == usuario.Id).FirstOrDefaultAsync();
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL

                if (tokenExistente == null)
                {
                    respuesta.HttpCode = HttpStatusCode.NotFound;
                    respuesta.Error = ErroresUsuario.RecursoNoEncontrado("Token");
                }
                else
                {
                    respuesta.Payload = Extensiones.AResultadoTokenloginLess(Guid.Parse(tokenExistente.UsuarioId), tokenExistente.Token);
                }
            }
            else
            {
                respuesta.HttpCode = HttpStatusCode.NotFound;
                respuesta.Error = ErroresUsuario.RecursoNoEncontrado("Dispositivo");
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener token loginless para el dispositivo {DispositivoId}", dispositivoId);
            respuesta.Error = ex.ToError();
            return respuesta;
        }
    }

    private List<string>? ObtieneRolesSistema()
    {
        string key = "sysroles";
        List<string>? roles = cache.LeeSerializado<List<string>>(key);
        if (roles == null)
        {
#pragma warning disable CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
            roles = roleManager.Roles == null ? [] : roleManager.Roles.Select(r => r.NormalizedName!.ToLower()).ToList();
#pragma warning restore CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
            if (roles != null)
            {
                cache.AlmacenaSerializadoSliding(key, roles);
            }
        }
        return roles;
    }

    public async Task<RespuestaPayload<RespuestaTokenVinculacion>> ObtieneTokenVinculacion(string DeviceId)
    {
        logger.LogDebug("ServicioUsuarios-ObtieneTokenVinculacion");
        var r = new RespuestaPayload<RespuestaTokenVinculacion>();
        try
        {
            logger.LogDebug("ServicioUsuarios-ObtieneTokenVinculacion buscando token existente");
            var tokenExistente = await db.TokensVinculacion.FindAsync(DeviceId);

            if (tokenExistente != null)
            {
                logger.LogDebug("ServicioUsuarios-ObtieneTokenVinculacion eliminando token");
                db.TokensVinculacion.Remove(tokenExistente);
                await db.SaveChangesAsync();
            }
            logger.LogDebug("ServicioUsuarios-ObtieneTokenVinculacion generando nuevo token");
            string token;
            do
            {
                token = await GeneraToken();
            }
            while (await db.TokensVinculacion.AnyAsync(e => e.Token == token));


            var nuevoToken = new TokenVinculacion { DeviceId = DeviceId, Token = token, Activado = false };

            db.TokensVinculacion.Add(nuevoToken);

            await db.SaveChangesAsync();
            logger.LogDebug("ServicioUsuarios-ObtieneTokenVinculacion token generador {Token}", nuevoToken);
            r.Ok = true;
            r.HttpCode = HttpStatusCode.OK;
            r.Payload = new RespuestaTokenVinculacion() { Fecha = nuevoToken.Fecha, Token = nuevoToken.Token };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios - ObtieneTokenVinculacion Error al obtener el token {Mensaje}", ex.Message);
        }

        return r;
    }

    private async Task EliminarTokensCaducados()
    {
        var caducados = await db.TokensVinculacion.Where(e => e.Fecha < DateTime.Now).ToListAsync();
        db.TokensVinculacion.RemoveRange(caducados);
        await db.SaveChangesAsync();
    }

    private async Task<string> GeneraToken()
    {
        await EliminarTokensCaducados();

        const string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string consonantes = "BCDFGHJKLMNPQRSTVWXYZ";

        var random = new Random();
        var token = new char[4];
        for (int i = 0; i < token.Length; i++)
        {
            if (i < 2)
            {
                token[i] = consonantes[random.Next(consonantes.Length)];
            }
            else
            {
                token[i] = letras[random.Next(letras.Length)];
            }
        }

        return new string(token);
    }

    public async Task<Respuesta> VerificarActivacion(string dispositivoId, string token)
    {
        logger.LogDebug("ServicioUsuarios-ServicioUsuarios {DispositivoId} {Token}", dispositivoId, token);

        Respuesta respuesta = new Respuesta();
        try
        {
            logger.LogDebug("ServicioUsuarios-ServicioUsuarios verificando la existencia de un TokenVinculacion.");

            var tokenVinculacion = await db.TokensVinculacion.FirstOrDefaultAsync(e => e.DeviceId == dispositivoId && e.Fecha > DateTime.UtcNow && e.Token == token);

            if (tokenVinculacion is null || !tokenVinculacion.Activado)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_USUARIOS_TOKEN_VINCULACION_NO_ENCONTRADO,
                    Mensaje = "Token de vinculación no encontrado",
                    HttpCode = HttpStatusCode.NotFound
                };
                respuesta.HttpCode = HttpStatusCode.NotFound;
                logger.LogError("ServicioUsuarios-ServicioUsuarios no se encontró un TokenViculación {Error}.", respuesta.Error);
                return respuesta;
            }

            db.TokensVinculacion.Remove(tokenVinculacion);
            await db.SaveChangesAsync();

            respuesta.Ok = true;
            respuesta.HttpCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios - VerificarActivacion Error al verificar la activación {Mensaje}", ex.Message);
        }
        return respuesta;
    }

    public async Task<RespuestaPayload<List<ParIdTexto>>> ObtieneUsuariosDeIds(List<string> ids)
    {
        logger.LogDebug("ServicioUsuarios-ObtieneUsuariosDeIds");
        var r = new RespuestaPayload<List<ParIdTexto>>();
        try
        {
            var usuarios = await db.Users.Where(u => ids.Contains(u.Id)).ToListAsync();

            r.Payload = [.. usuarios.Select(u => new ParIdTexto() { Id = u.Id, Texto = u.Nombre ?? u.Email ?? "" })];

            await db.SaveChangesAsync();
            logger.LogDebug("ServicioUsuarios-ObtieneUsuariosDeIds usuarios encontrados {Total}", r.Payload.Count());
            r.Ok = true;
            r.HttpCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios - ObtieneUsuariosDeIds Error al obtener usuarios {Mensaje}", ex.Message);
            r.Error = ErroresUsuario.Desconocido(ex.Message, "ObtieneUsuariosDeIds");
            r.HttpCode = HttpStatusCode.BadRequest;

        }

        return r;
    }

    public async Task<RespuestaPayload<DTORecuperaContrasena>> RecuperaContrasenaEmail(string Email)
    {
        logger.LogDebug("ServicioUsuarios-RecuperaContrasenaEmail {Email}", Email);
        RespuestaPayload<DTORecuperaContrasena> respuesta = new();
        try
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user is null)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_USUARIO_NO_ENCONTRADO,
                    Mensaje = "No se encontró el usuario.",
                    HttpCode = HttpStatusCode.NotFound
                };
                respuesta.HttpCode = HttpStatusCode.NotFound;
                logger.LogError("ServicioUsuarios-RecuperaContraseñaEmail {Error}.", respuesta.Error);
                return respuesta;
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user!);

            respuesta.Payload = new DTORecuperaContrasena()
            {
                Email = user!.Email,
                UserName = user!.UserName,
                TokenRecuperacion = token
            };
      
            respuesta.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios - RecuperaContrasenaEmail Error al recuperar la contraseña {Mensaje}", ex.Message);
        }

        return respuesta;
    }

    public async Task<Respuesta> EstablaceContrasenaToken(ActualizaContrasena actualizaContrasena)
    {
        logger.LogDebug("ServicioUsuarios-EstablaceContrasenaToken {email}", actualizaContrasena.Email);
        Respuesta respuesta = new();
        try
        {
            var user = await userManager.FindByEmailAsync(actualizaContrasena.Email!);
            if (user is null)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_USUARIO_NO_ENCONTRADO,
                    Mensaje = "No se encontró el usuario.",
                    HttpCode = HttpStatusCode.NotFound
                };
                respuesta.HttpCode = HttpStatusCode.NotFound;
                logger.LogError("ServicioUsuarios-RecuperaContraseñaEmail {Error}.", respuesta.Error);
                return respuesta;
            }

            var result = await userManager.ResetPasswordAsync(user, actualizaContrasena.TokenRecuperacion, actualizaContrasena.NuevaContrasena);


            if (!result.Succeeded)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Codigo = CodigosError.IDENTITY_SERVICIO_USUARIO_ERROR_RECUPERA_CONTRASENA,
                    Mensaje = "Ocurrió un error inesperado al actualizar la contraseña nueva.",
                    HttpCode = HttpStatusCode.InternalServerError
                };
                respuesta.HttpCode = HttpStatusCode.InternalServerError;
                logger.LogError("ServicioUsuarios-RecuperaContraseñaEmail {Error}.", respuesta.Error);
                return respuesta;
            }

            respuesta.Ok = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioUsuarios - EstablaceContrasenaToken Error al establecer la contraseña {Mensaje}", ex.Message);
        }

        return respuesta;
    }
}
