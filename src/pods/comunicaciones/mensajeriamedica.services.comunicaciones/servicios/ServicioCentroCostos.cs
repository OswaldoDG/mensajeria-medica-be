using comunes.extensiones;
using comunes.proxies.proxygenerico;
using comunes.respuestas;
using mensajeriamedica.model.comunicaciones.centroscostos;
using mensajeriamedica.model.comunicaciones.centroscostos.dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace mensajeriamedica.services.comunicaciones.servicios;

public class ServicioCentroCostos(ILogger<ServicioCentroCostos> logger, DbContextMensajeria db, IProxyGenericoInterservicio proxy) : IServicioCentroCostos
{

    public async Task<RespuestaPayload<CentroCostos>> CreaCentroCostos(string nombre)
    {
        // Verificar si no existe un homónimo activo, si existe devolver Conflict.
        logger.LogDebug("ServicioCentroCostros - CreaCentroCostos.");
        RespuestaPayload<CentroCostos> respuesta = new RespuestaPayload<CentroCostos>();
        try
        {
            var existe = await db.CentroCostos.FirstOrDefaultAsync(c => c.Nombre.Equals(nombre) && !c.Eliminado);

            if (existe != null )
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "Error en la creación del Centro por existencia de Homónimo.",
                    HttpCode = System.Net.HttpStatusCode.Conflict,
                    Codigo = "ExisteHomónimo"
                };
                logger.LogWarning("ServicioCentroCostros-CreaCentroCostos Error al realizar la creación del centro {Mensaje}", respuesta.Error.Mensaje);
                return respuesta;
            }

            var centro = new CentroCostos()
            {
                Nombre = nombre
            };

            db.CentroCostos.Add(centro);
            await db.SaveChangesAsync();
            respuesta.Payload = centro;
            respuesta.Ok = true;
            logger.LogDebug("ServicioCentroCostros-CreaCentroCostos exitosa.");
            return respuesta;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ServicioCentroCostros-CreaCentroCostos Error al realizar la creación del centro {Mensaje}.", ex.Message);
            respuesta.Error = ex.ToError();
            return respuesta;
        }
    }

    public async Task<Respuesta> ActualizaCentroCostos(int id, string nombre)
    {
        logger.LogDebug("ServicioCentroCostros - ActualizaCentroCostos.");
        Respuesta respuesta = new Respuesta();
        try
        {
            if (!await CentroCostosActivo(id))
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = $"No existe un CentroCostos activo con Id {id}",
                    Codigo = "NoEncontrado",
                    HttpCode = System.Net.HttpStatusCode.NotFound
                };
                logger.LogWarning("ServicioCentroCostros-ActualizaCentroCostos Error {Mensaje}", respuesta.Error.Mensaje);
                return respuesta;
            }

            // Verificar si no existe un homónimo activo, si existe devolver Conflic con Id diferente al que se intenta actualizar.
            var homonimo = await db.CentroCostos.FirstOrDefaultAsync(c => c.Nombre == nombre && !c.Eliminado && c.Id != id);

            if (homonimo != null)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = $"Existe un homónimo activo con Id {homonimo.Id}",
                    Codigo = "Homónimo encontrado al actualizar el CentroCostos",
                    HttpCode = System.Net.HttpStatusCode.Conflict,
                };
                logger.LogWarning("ServicioCentroCostros-ActualizaCentroCostos Error al realizar la actualizacion de la unidad de costos {Mensaje}.", respuesta.Error.Mensaje);
                return respuesta;
            }

            // Actualizar sólo el nombre del centro de costos
            var centro = await db.CentroCostos.FirstOrDefaultAsync(c => c.Id == id);
            centro.Nombre = nombre;
            db.CentroCostos.Update(centro);
            await db.SaveChangesAsync();
            logger.LogDebug("ServicioCentroCostos - ActualizaCentroCostos exitosa.");
            respuesta.Ok = true;
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-ActualizaCentroCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    public async Task<Respuesta> ActualizaUnidadCostos(int centroCostosId, int id, string nombre, string clave)
    {
        Respuesta respuesta = new Respuesta();
        logger.LogDebug("ServicioCentroCostros - ActualizaUnidadCostos.");
        try
        {
            if (!await CentroCostosActivo(centroCostosId))
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = $"No existe un CentroCostos activo con Id {id}",
                    Codigo = "NoEncontrado",
                    HttpCode = System.Net.HttpStatusCode.NotFound
                };
                logger.LogWarning("ServicioCentroCostros-ActualizaCentroCostos Error {Mensaje}", respuesta.Error.Mensaje);
                return respuesta;
            }

            // VErificar si hay algun registro con la clave antigua en Mensajes.SucursalId
            // y actualizarlo con la nueva clave SOLO SI NO EXISTEN en la tabla mensajes en el campo.

            var unidadCostos = await db.UnidadCostos.FirstOrDefaultAsync(c => c.Id == id);

            var mensajes = await db.Mensajes.AnyAsync(m => m.SucursalId.Equals(unidadCostos.Clave));

            // Si hay algiun registro devolver Conflict.
            if (mensajes)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "Unidad de Costos con registros de mensajes no es posible la actualización",
                    Codigo = "UnidadCostosConRegistros",
                    HttpCode = System.Net.HttpStatusCode.Conflict
                };

                logger.LogWarning("ServicioCentroCostros - ActualizaUnidadCostos mensajes existentes no es posible actualizar la unidad de costos.");
                return respuesta;
            }

            // Actualizar sólo el nombre y clave de la unidad de costos.
            unidadCostos.Nombre = nombre;
            unidadCostos.Clave = clave;

            db.UnidadCostos.Update(unidadCostos);
            await db.SaveChangesAsync();

            respuesta.Ok = true;
            logger.LogDebug("ServicioCentroCostros - ActualizaUnidadCostos existosa.");
            return respuesta;

        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-ActualizaUnidadCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    public async Task<Respuesta> AgregaUsuarioCentroCostos(int centroCostosId, Guid UsuarioID)
    {
        logger.LogDebug("ServicioCentroCostos - AgregaUsuarioCentroCostos.");
        Respuesta respuesta = new Respuesta();
        try
        {
            if (!await CentroCostosActivo(centroCostosId))
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "El centro no se encuentra activo para el registro del usuario",
                    Codigo = "CentroInactivo",
                    HttpCode = System.Net.HttpStatusCode.Conflict
                };

                logger.LogWarning("ServicioCentroCostros - AgregaUsuarioCentroCostos mensajes existentes no es posible actualizar la unidad de costos.");
                return respuesta;
            }

            var user = new UsuarioCentroCostos()
            {
                CentroCostosId = centroCostosId,
                UsuarioId = UsuarioID
            };

            db.UsuarioCentrosCostos.Add(user);
            await db.SaveChangesAsync();
            respuesta.Ok = true;
            logger.LogDebug("ServicioCentroCostos - AgregaUsuarioCentroCostos existoso.");
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-AgregaUsuarioCentroCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    public async Task<Respuesta> EliminaUsuarioCentroCostos(int centroCostosId, Guid UsuarioID)
    {
        logger.LogDebug("ServicioCentrosCostos - EliminaUsuarioCentroCostos");
        Respuesta respuesta = new Respuesta();
        try
        {
            if (!await CentroCostosActivo(centroCostosId))
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "El centro no se encuentra activo para el registro del usuario",
                    Codigo = "CentroInactivo",
                    HttpCode = System.Net.HttpStatusCode.Conflict
                };

                logger.LogWarning("ServicioCentroCostros - EliminaUsuarioCentroCostos {Mensaje}.", respuesta.Error.Mensaje);
                return respuesta;
            }

            var user = await db.UsuarioCentrosCostos.FirstOrDefaultAsync(user => user.UsuarioId == UsuarioID);

            if (user == null)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "El usuario no se encontró",
                    Codigo = "CentroInactivo",
                    HttpCode = System.Net.HttpStatusCode.NotFound
                };

                logger.LogWarning("ServicioCentroCostos - EliminaUsuarioCentroCostos {Mensaje}", respuesta.Error.Mensaje);
                return respuesta;
            }

            db.UsuarioCentrosCostos.Remove(user);
            await db.SaveChangesAsync();
            respuesta.Ok = true;
            logger.LogDebug("ServicioCentroCostos - EliminaUsuarioCentroCostos exitoso.");
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-EliminaUsuarioCentroCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    public async Task<RespuestaPayload<UnidadCostos>> CreaUnidadCostos(int centroCostosId, string nombre, string clave)
    {
        logger.LogDebug("ServicioCentroCostos - CreaUnidadCostos");
        RespuestaPayload<UnidadCostos> respuesta = new RespuestaPayload<UnidadCostos>();
        try
        {
            if (!await CentroCostosActivo(centroCostosId))
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "El centro no se encuentra activo para el registro del usuario",
                    Codigo = "CentroInactivo",
                    HttpCode = System.Net.HttpStatusCode.Conflict
                };

                logger.LogWarning("ServicioCentroCostros - CreaUnidadCostos {Mensaje}.", respuesta.Error.Mensaje);
                return respuesta;
            }

            // Realizar la operacón solo si el centro de costos está activo.
            // VErifica que no haya unidads con CLAVE IDENTICA, si pueden existir con el mismo nombre pero no con la misma clave, si existe devolver Conflict.

            var homonimoClave = await db.UnidadCostos.AnyAsync(u => u.Clave.Equals(clave));

            if (homonimoClave)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "Unidad de Costos con misma clave existente.",
                    Codigo = "ClaveExistente",
                    HttpCode = System.Net.HttpStatusCode.Conflict
                };

                logger.LogWarning("ServicioCentroCostros - CreaUnidadCostos {Mensaje}.", respuesta.Error.Mensaje);
                return respuesta;
            }

            var unidad = new UnidadCostos()
            {
                CentroCostosId = centroCostosId,
                Clave = clave,
                Nombre = nombre
            };

            db.UnidadCostos.Add(unidad);
            await db.SaveChangesAsync();
            respuesta.Ok = true;
            respuesta.Payload = unidad;
            logger.LogDebug("ServicioCentroCostos - CreaUnidadCostos exitoso.");
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-CreaUnidadCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    public async Task<Respuesta> EliminaCentroCostos(int id)
    {
        logger.LogDebug("ServicioCentroCostos - EliminaCentroCostos");
        Respuesta respuesta = new Respuesta();
        try
        {
            // MArca Eliminado = true.
            var centroCostos = await db.CentroCostos.FirstOrDefaultAsync(c => c.Id == id);

            if (centroCostos == null)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "Centro de Costos no encontrado.",
                    Codigo = "CentroCostosNoEncontrado",
                    HttpCode = System.Net.HttpStatusCode.NotFound
                };

                logger.LogWarning("ServicioCentroCostos - EliminaCentroCostos {Mensaje}.", respuesta.Error.Mensaje);
                return respuesta;
            }

            centroCostos.Eliminado = true;
            db.CentroCostos.Update(centroCostos);
            await db.SaveChangesAsync();
            logger.LogDebug("ServicioCentroCostos - EliminaCentroCostos exitoso");
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-EliminaUsuarioCentroCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    public async Task<Respuesta> EliminaUnidadCostos(int centroCostosId, int id)
    {
        logger.LogDebug("ServicioCentroCostos - ElimnaUnidadCostos");
        Respuesta respuesta = new Respuesta();
        try
        {
            // Realizar la operacón solo si el centro de costos está activo.
            if (!await CentroCostosActivo(centroCostosId))
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "El centro no se encuentra activo para el registro del usuario",
                    Codigo = "CentroInactivo",
                    HttpCode = System.Net.HttpStatusCode.Conflict
                };

                logger.LogWarning("ServicioCentroCostros - EliminaUnidadCostos {Mensaje}.", respuesta.Error.Mensaje);
                return respuesta;
            }

            // Puede eliminarla solo si no hay Mensajes.SucursalId con la clave de la unidad de costos, si hay algiun registro devolver Conflict.
            var unidad = await db.UnidadCostos.FirstOrDefaultAsync(u => u.Id == id);

            var mensajes = await db.Mensajes.AnyAsync(m => m.SucursalId == unidad.Clave);

            if (mensajes)
            {
                respuesta.Error = new ErrorProceso()
                {
                    Mensaje = "Unidad de Costos con mensajes no es posible la eliminación.",
                    Codigo = "UnidadCostosConMensajes",
                    HttpCode = System.Net.HttpStatusCode.Conflict
                };
                logger.LogWarning("ServicioCentroCostros - EliminaUnidadCostos {Mensaje}.", respuesta.Error.Mensaje);
                return respuesta;
            }

            db.UnidadCostos.Remove(unidad);
            await db.SaveChangesAsync();
            respuesta.Ok = true;
            logger.LogDebug("ServicioCentroCostos - EliminaUnidadCostos exitosa.");
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-EliminaUsuarioCentroCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    /// <summary>
    /// VErifica si el centro de costos se ecuentra activo.
    /// </summary>
    /// <param name="id">ID del centro de costos.</param>
    /// <returns>Si el centro de costos está activo.</returns>
    private async Task<bool> CentroCostosActivo(int id)
    {
        // Si el centro por id existe y no esta Eliminado devolver true.
        return await db.CentroCostos.AnyAsync(c => c.Id == id && !c.Eliminado);
    }

    public async Task<RespuestaPayload<List<CentroCostos>>> ObtieneCentroCostos(bool incluirEliminados)
    {
        // Devuelve la lista de centros de costos, si incluirEliminados es false sólo devuelve los que Eliminado = false, si incluirEliminados es true devuelve todos.
        logger.LogDebug("ServicioCentroCostos - ObtieneCentroCostos");
        RespuestaPayload<List<CentroCostos>> respuesta = new RespuestaPayload<List<CentroCostos>>();
        try
        {
            if (incluirEliminados)
            {
                var centroCostos = await db.CentroCostos.ToListAsync();
                respuesta.Ok = true;
                respuesta.Payload = centroCostos;
            }
            else
            {
                var centroCostosActivos = await db.CentroCostos.Where(c => !c.Eliminado).ToListAsync();
                respuesta.Ok = true;
                respuesta.Payload = centroCostosActivos;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-EliminaUsuarioCentroCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }

    public async Task<RespuestaPayload<List<DtoUsuario>>> ObtieneListaUsuarios()
    {
        logger.LogDebug("ServicioCentroCostos - ObtieneListaUsuarios");
        RespuestaPayload<List<DtoUsuario>> respuesta = new RespuestaPayload<List<DtoUsuario>>();
        try
        {
            var respuestaProxy = await proxy.JsonRespuestaSerializada("identidad", "usuarios/lista", "ObtieneListaUsuarios", VerboHttp.GET, null);

            if (!respuestaProxy.Ok)
            {
                logger.LogError("ServicioCentroCostos - ObtieneListaUsuarios error al obtener la lista de usuarios  {Error}.", $"{respuestaProxy.HttpCode} {respuestaProxy.Error?.Codigo} {respuestaProxy.Error?.Mensaje}");

                respuesta.Error = respuestaProxy.Error;
                respuesta.Error!.Codigo = "Error al obtener la lista de usaurios";
                respuesta.HttpCode = HttpStatusCode.InternalServerError;
                return respuesta;
            }

            respuesta.Payload = JsonConvert.DeserializeObject<List<DtoUsuario>>(respuestaProxy.Payload);
            respuesta.Ok = true;
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Error = new ErrorProceso()
            {
                Mensaje = ex.Message,
                Codigo = "ErrorInterno",
                HttpCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError(ex, "ServicioCentroCostros-EliminaUsuarioCentroCostos Error {Mensaje}", ex.Message);
            return respuesta;
        }
    }
}
