using mensajeriamedica.model.comunicaciones.hl7;

namespace mensajeriamedica.services.comunicaciones.interpretes;

/// <summary>
/// Interfaz para interpretar mensajes HL7.
/// </summary>
public interface IInterpreteHL7
{
    ContactoEstudio? ObtieneContacto(string hl7data);
}
