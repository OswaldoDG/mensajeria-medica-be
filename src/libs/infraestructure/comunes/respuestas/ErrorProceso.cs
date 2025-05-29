using System.Net;

namespace comunes.respuestas;

/// <summary>
/// Define un error de validacion.
/// </summary>
public class ErrorProceso
{
    /// <summary>
    /// Codigo único del error.
    /// </summary>
    public string? Codigo { get; set; }

    /// <summary>
    /// Mensaje para lectura human.
    /// </summary>
    public string? Mensaje { get; set; }

    /// <summary>
    /// Nombre de la propiedad orígen del eerror.
    /// </summary>
    public string? Propiedad { get; set; }

    /// <summary>
    /// REsultado REST.
    /// </summary>
    public HttpStatusCode HttpCode { get; set; } = HttpStatusCode.NotImplemented;

}
