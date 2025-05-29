using System.Collections.Specialized;
using System.Reflection;
using comunes.respuestas;

namespace comunes.extensiones;

public static class Extensiones
{
    public static ErrorProceso ToError(this Exception ex, string? codigo = null)
    {
        return new ErrorProceso()
        {
            Codigo = codigo,
            HttpCode = System.Net.HttpStatusCode.InternalServerError,
            Mensaje = ex.Message,
            Propiedad = null
        };
    }
}
