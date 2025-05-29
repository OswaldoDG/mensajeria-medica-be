using System.Net;

namespace comunes.respuestas;

/// <summary>
/// Respuesta base sin payload.
/// </summary>
public class Respuesta
{
    private ErrorProceso? _error = null;
    public bool Ok { get; set; } = false;

    public ErrorProceso? Error
    {
        get
        {
            return _error;
        }

        set
        {
            if (value != null)
            {
                HttpCode = value!.HttpCode;
            }

            Ok = value == null;
            if (Ok) this.HttpCode = HttpStatusCode.OK;
            _error = value;
        }
    }

    public HttpStatusCode HttpCode { get; set; } = HttpStatusCode.NotImplemented;
}

public class RespuestaBoolean : Respuesta
{
    public RespuestaBoolean()
    {
        Ok = false;
        HttpCode = HttpStatusCode.NotImplemented;
    }

    /// <summary>
    /// Resultado de la operacion booleana.
    /// </summary>
    public bool Resultado { get; set; }
}

/// <summary>
/// Respuesta generica.
/// </summary>
/// <typeparam name="T">Tipo del objeto esperado como respuesta.</typeparam>
public class RespuestaPayload<T> : Respuesta
    where T : class
{
    public RespuestaPayload()
    {
        Ok = false;
        HttpCode = HttpStatusCode.NotImplemented;
    }

    private T? _payload;

    /// <summary>
    /// Carga util de la respuesta, si se asigna un valor diferente de nulo se establece OK = true.
    /// </summary>
    public T? Payload
    {
        get
        {
            return _payload;
        }

        set
        {
            Ok = value != null;
            _payload = value;
        }
    }
}

/// <summary>
/// Respuesta serializada en formato JSON.
/// </summary>
public class RespuestaPayloadJson : Respuesta
{
    public RespuestaPayloadJson()
    {
        Ok = false;
        HttpCode = HttpStatusCode.NotImplemented;
    }

    private string? _payload;

    /// <summary>
    /// Carga util de la respuesta, si se asigna un valor diferente de nulo se establece OK = true.
    /// </summary>
    public string? Payload
    {
        get
        {
            return _payload;
        }

        set
        {
            Ok = !string.IsNullOrEmpty(value);
            _payload = value;
        }
    }
}