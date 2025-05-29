using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using comunes.respuestas;
using comunes.servicios.crm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace comunes;

public class ControllerUsoInterno(ILogger logger) : ControllerBase
{
    [NonAction]
    protected ActionResult ActionFromCode(HttpStatusCode code, string? Error = null, string? codigoError = null)
    {
        logger.LogDebug("{Code} {Mensaje}", code, Error);
        string contenido = $"{codigoError}: {Error}";
        return StatusCode(code.GetHashCode(), contenido);
    }

    [NonAction]
    protected ActionResult ActionFromCode(HttpStatusCode code, ErrorProceso? error)
    {
        logger.LogDebug("{Code} {Mensaje}", code, error?.Mensaje);
        string contenido = $"{error?.Codigo}: {error?.Mensaje}";
        return StatusCode(code.GetHashCode(), contenido);
    }


    protected Guid? UsuarioGuid
    {
        get
        {
            string? id = this.UsuarioId;
            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out var gId))
            {
                return gId;
            }

            return null;
        }
    }

    protected string? UsuarioId
    {
        get
        {
            try
            {
                string? jwt = Request?.Headers.Authorization;
                if (!string.IsNullOrEmpty(jwt) && jwt.IndexOf("Bearer", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(jwt.Split(' ')[1]);
                    var tokenS = jsonToken as JwtSecurityToken;
                    return tokenS?.Subject;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

            return null;
        }
    }

    protected bool TieneRolesOr(List<string> roles)
    {
        var lista = this.Claims.Where(c => c.Type.ToLower() == "role").ToList();
        foreach (var rol in roles)
        {
            if (lista.Any(c => c.Value.ToLower() == rol.ToLower()))
            {
                return true;
            }
        }

        return false;
    }

    protected bool TieneRolesAnd(List<string> roles)
    {
        var lista = this.Claims.Where(c => c.Type.ToLower() == "role").ToList();
        foreach (var rol in roles)
        {
            if (!lista.Any(c => c.Value.ToLower() == rol.ToLower()))
            {
                return false;
            }
        }

        return true;
    }

    protected List<string> Roles()
    {
        List<string> roles = new List<string>();
        foreach (var rol in Claims.Where(c => c.Type.ToLower() == "role").ToList())
        {
            roles.Add(rol.Value);
        }
        return roles;
    }

    protected List<Claim> Claims
    {
        get
        {
            try
            {
                string? jwt = Request?.Headers.Authorization;
                if (!string.IsNullOrEmpty(jwt) && jwt.IndexOf("Bearer", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(jwt.Split(' ')[1]);
                    var tokenS = jsonToken as JwtSecurityToken;
                    return tokenS?.Claims != null ? tokenS.Claims.ToList() : [];

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return [];
        }
    }
}
