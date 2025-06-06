using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.comunicaciones.hl7;

/// <summary>
/// DAtos del contacto del estudio.
/// </summary>
[ExcludeFromCodeCoverage]
public class ContactoEstudio
{
    /// <summary>
    /// Determina si el extractor ha obtenido todos los datos necesario para el envío.
    /// </summary>
    public bool DatosValidos { get; set; } = false;

    /// <summary>
    /// Teléfono del contacto.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Nombre del cotnacto.
    /// </summary>
    public string? NombreContacto { get; set; } // Juntar apellidos y nombre

    /// <summary>
    /// Pais del contacto.
    /// </summary>
    public string? Pais { get; set; }


    /// <summary>
    /// URL para la consulta del estudio.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Identificador de la sucursal que publica el mensaje.
    /// </summary>
    public string? SucursalId { get; set; } // Sucursal /clinica
}
