using comunes.extensiones;
using comunes.proxies.proxygenerico;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace comunes.servicios.crm;

/// <summary>
/// Servicio interproceso para consulat de datos del CRM.
/// </summary>
public class ServicioInterprocesoSesion(ILogger<ServicioInterprocesoSesion> logger, IDistributedCache cache, IProxyGenericoInterservicio proxyGenerico) : IServicioInterprocesoSesion
{
    private const string ClaveCacheCuentaFiscal = "cf";
    private const string ClaveCacheRolesUsuario = "cru";
    private const string ClaveCacheUsuarioCuentaFiscal = "cfu";
    private const string ValorNulo = "N";

    public async Task<bool> ExisteCuentaFiscal(Guid id)
    {
        try
        {
            logger.LogDebug("ServicioInterprocesoCRM-ExisteCuentaFiscal {Id}", id);
            string clave = $"{ClaveCacheCuentaFiscal}{id}";
            var cuenta = cache.Lee(clave);
            if (!string.IsNullOrEmpty(cuenta) )
            {
                logger.LogDebug("ServicioInterprocesoCRM-ExisteCuentaFiscal valor en cache {Valor}", cuenta);
                return cuenta != ValorNulo;
            }
            else
            {
                var result = await proxyGenerico.JsonRespuestaSerializada("crm", $"/crm/cuentafiscal/{id}", $"Localizar cuenta fiscal {id}", VerboHttp.GET, null);
                if (result.Ok)
                {
                    cache.AlmacenaSerializadoSliding(clave, result.Payload!);
                    return true;
                }
                else
                {
                    cache.AlmacenaSerializadoSliding(clave, ValorNulo);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioInterprocesoCRM - ExisteCuentaFiscal Error localizar la cuenta fiscal cuenta {Mensaje}", ex.Message);
            throw;
        }
    }

    public async Task<bool> ExisteUsuarioCuentaFiscal(Guid cuentaFiscalId, Guid usuarioId, List<string>? roles)
    {
        logger.LogDebug("ServicioInterprocesoCRM-ExisteUsuarioCuentaFiscal {UsuarioId}@{CuentaId}", usuarioId, cuentaFiscalId);
        AsociacionCuentaFiscal? asociacionCuenta = await this.CacheCuentaFiscalUsuario(usuarioId, cuentaFiscalId);

        if (asociacionCuenta != null)
        {
            // Si hay roles para verificar retorna false si falta alguno.
            if (roles != null && roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    if (!asociacionCuenta.Roles.Contains(role))
                    {
                        return false;
                    }
                }
            }

            // Si tiene todos los roles o no se especifican require que la cuenta se encuentre activa.
            return asociacionCuenta.Activa;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> EsPrimarioCuentaFiscal(Guid cuentaFiscalId, Guid usuarioId)
    {
        logger.LogDebug("ServicioInterprocesoCRM-ExisteUsuarioCuentaFiscal {UsuarioId}@{CuentaId}", usuarioId, cuentaFiscalId);
        AsociacionCuentaFiscal? asociacionCuenta = await this.CacheCuentaFiscalUsuario(usuarioId, cuentaFiscalId);

        if (asociacionCuenta != null)
        {
            return asociacionCuenta.TipoCuenta == TipoCuenta.Primaria;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene una cunta fiscal para un usuario.
    /// </summary>
    /// <param name="usuarioId">Id del usuario.</param>
    /// <param name="cuentaFiscalId">Id de la cuenta fiscal.</param>
    /// <returns>Asociacion o nulo.</returns>
    private async Task<AsociacionCuentaFiscal?> CacheCuentaFiscalUsuario(Guid usuarioId, Guid cuentaFiscalId)
    {
        AsociacionCuentaFiscal? asociacionCuenta = null;
        try
        {
            logger.LogDebug("ServicioInterprocesoCRM-Obteniendo CuentaFiscalUsuario {UsuarioId}@{CuentaId}", usuarioId, cuentaFiscalId);
            string clave = $"{ClaveCacheUsuarioCuentaFiscal}{cuentaFiscalId}{usuarioId}";
            var usuarioCuenta = cache.Lee(clave);
            if (!string.IsNullOrEmpty(usuarioCuenta))
            {
                if (usuarioCuenta != ValorNulo)
                {
                    asociacionCuenta = JsonConvert.DeserializeObject<AsociacionCuentaFiscal>(usuarioCuenta);
                }
            }
            else
            {
                var result = await proxyGenerico.JsonRespuestaSerializada("crm", $"/crm/cuentafiscal/{cuentaFiscalId}/miembro/{usuarioId}", $"Localizar usuario {usuarioId} en cuenta fiscal {cuentaFiscalId}", VerboHttp.GET, null);
                if (result.Ok)
                {
                    asociacionCuenta = JsonConvert.DeserializeObject<AsociacionCuentaFiscal>(result.Payload!);
                    asociacionCuenta!.Roles = await this.ObtieneRolesUSuario(usuarioId);
                    cache.AlmacenaSerializadoAbsoluto(clave, JsonConvert.SerializeObject(asociacionCuenta));
                }
                else
                {
                    cache.AlmacenaSerializadoAbsoluto(clave, ValorNulo);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioInterprocesoCRM - ExisteCuentaFiscal Error localizar la cuenta fiscal cuenta {Mensaje}", ex.Message);
        }

        return asociacionCuenta;
    }

    /// <summary>
    /// Obtiene los roles del usuario.
    /// </summary>
    /// <param name="id">Id del usuario.</param>
    /// <returns>Lista de roles asociados.</returns>
    private async Task<List<string>> ObtieneRolesUSuario(Guid id)
    {
        try
        {
            logger.LogDebug("ServicioInterprocesoCRM-ObtieneRolesUSuario {Id}", id);
            string clave = $"{ClaveCacheRolesUsuario}{id}";
            string? roles = cache.Lee(clave);
            if (!string.IsNullOrEmpty(roles))
            {
                logger.LogDebug("ServicioInterprocesoCRM-ObtieneRolesUSuario valor en cache {Valor}", roles);
            }
            else
            {
                var result = await proxyGenerico.JsonRespuestaSerializada("identidad", $"/usuario/{id}/roles", $"Obtener roles del usuario {id}", VerboHttp.GET, null);
                if (result.Ok)
                {
                    roles = result.Payload!;
                    logger.LogDebug("ServicioInterprocesoCRM-ObtieneRolesUSuario valor API {Valor}", roles);
                    cache.AlmacenaSerializadoSliding(clave, result.Payload!);
                }
            }

            if (!string.IsNullOrEmpty(roles))
            {
                return JsonConvert.DeserializeObject<List<string>>(roles!) ?? [];
            }

            return [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioInterprocesoCRM - ObtieneRolesUSuario Error localizar la cuenta fiscal cuenta {Mensaje}", ex.Message);
            throw;
        }
    }
}
