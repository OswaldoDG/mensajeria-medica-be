using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace comunes.extensiones;

/// <summary>
/// Estansiones de cache.
/// </summary>
public static class ExtensionesCache
{
    /// <summary>
    /// Almacena un valor que expira n minutos despues de manera sliding.
    /// </summary>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    /// <param name="data">Datos.</param>
    /// <param name="minutos">Minutos.</param>
    public static void AlmacenaSerializadoSliding(this IDistributedCache cache, string key, object data, int minutos = 5)
    {
        if (cache != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            if (cache.Get(key) != null)
            {
                cache.Remove(key);
            }

            cache.Set(key, bytes, new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(minutos) });
        }
    }

    /// <summary>
    /// Almacena un valor que expira n minutos despues de manera absoluta.
    /// </summary>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    /// <param name="data">Datos.</param>
    /// <param name="minutos">Minutos.</param>
    public static void AlmacenaSerializadoAbsoluto(this IDistributedCache cache, string key, object data, int minutos = 5)
    {
        if (cache != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            if (cache.Get(key) != null)
            {
                cache.Remove(key);
            }

            cache.Set(key, bytes, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutos) });
        }
    }

    /// <summary>
    /// Almacena un valor que expira n minutos despues de manera absoluta.
    /// </summary>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    /// <param name="data">Datos.</param>
    /// <param name="minutos">Minutos.s</param>
    public static void AlmacenaAbsoluto(this IDistributedCache cache, string key, string data, int minutos = 5)
    {
        if (cache != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            if (cache.Get(key) != null)
            {
                cache.Remove(key);
            }

            cache.Set(key, bytes, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutos) });
        }
    }

    /// <summary>
    /// Almacena un valor que expira n minutos despues de manera sliding.
    /// </summary>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    /// <param name="data">Datos.</param>
    /// <param name="minutos">Minutos.s</param>
    public static void AlmacenaSliding(this IDistributedCache cache, string key, string data, int minutos = 5)
    {
        if (cache != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            if (cache.Get(key) != null)
            {
                cache.Remove(key);
            }

            cache.Set(key, bytes, new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(minutos) });
        }
    }

    /// <summary>
    /// Elimina una entrada del cache.
    /// </summary>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    public static void Elimina(this IDistributedCache cache, string key)
    {
        if (cache != null && cache.Get(key) != null)
        {
            cache.Remove(key);
        }
    }

    /// <summary>
    /// Refresca una entrada del cache.
    /// </summary>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    public static void Refresca(this IDistributedCache cache, string key)
    {
        if (cache != null && cache.Get(key) != null)
        {
            cache.Refresh(key);
        }
    }

    /// <summary>
    /// Devulve un objeto a partir de una serializacion en cache.
    /// </summary>
    /// <typeparam name="T">Tipo de objeto a devolver.</typeparam>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    /// <returns>Nulo o el objeto.</returns>
    public static T? LeeSerializado<T>(this IDistributedCache cache, string key)
        where T : class
    {
        if (cache != null)
        {
            byte[]? bytes = cache!.Get(key);

            if (bytes != null)
            {
                return JsonConvert.DeserializeObject<T>(System.Text.Encoding.UTF8.GetString(bytes));
            }
        }

        return null;
    }

    /// <summary>
    /// Lee un contenido almacenado en cache.
    /// </summary>
    /// <param name="cache">Instancia de cache.</param>
    /// <param name="key">Clave.</param>
    /// <returns>Nulo o el contenido.</returns>
    public static string? Lee(this IDistributedCache cache, string key)
    {
        if (cache != null)
        {
            byte[]? bytes = cache!.Get(key);

            if (bytes != null)
            {
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
        }

        return null;
    }
}
