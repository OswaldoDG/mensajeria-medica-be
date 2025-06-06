using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.comunicaciones.mensajes;

namespace mensajeriamedica.model.comunicaciones.servidores;

/// <summary>
/// Servidor del servicio de mensajería.
/// </summary>
[ExcludeFromCodeCoverage]
public class Servidor
{
    /// <summary>
    /// Identificador del // Clave primaria.
    /// </summary>
    public long Id { get; set; } // Clave primaria

    /// <summary>
    /// Nombre del c.
    /// </summary>
    required public string Nombre { get; set; } // MaxLen 100

    /// <summary>
    /// Nombre del folder de FTP donde se almacenan los archivos del // Clave primaria.
    /// </summary>
    required public string FolderFtp { get; set; } // MaxLen 250

    /// <summary>
    /// Mensajes asociados al cliente.
    /// </summary>
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
    public List<Mensaje> Mensajes { get; set; }
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
}
