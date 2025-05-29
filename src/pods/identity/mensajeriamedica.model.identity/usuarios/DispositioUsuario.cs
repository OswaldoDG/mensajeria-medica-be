using System.Text.Json.Serialization;
using System.Xml.Serialization;
using mensajeriamedica.model.identity.registro;

namespace contabee.model.identity.usuarios;

/// <summary>
/// Mantiene la relación dispositivo usuario.
/// </summary>
public class DispositivoUsuario
{
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
    /// <summary>
    /// IDentificador único de la asociacion.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Identificador único del Usuario.
    /// </summary>
    public string UsuarioId { get; set; }

    /// <summary>
    /// Identificador único del Dispositivo.
    /// </summary>
    public string DispositivoId { get; set; }

    /// <summary>
    /// Fecha de la asociacion.
    /// </summary>
    public DateTime FechaAsociacion { get; set; } = DateTime.UtcNow;

    [XmlIgnore]
    [JsonIgnore]
    public ApplicationUser Usuario { get; set; }
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
}
