using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.comunicaciones.hl7;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.interpretes;

/// <summary>
/// Interprete de mensajes HL7 para extraer información de contacto del estudio.
/// </summary>
[ExcludeFromCodeCoverage]
public class InterpreteHL7(ILogger<InterpreteHL7> logger) : IInterpreteHL7
{
    public ContactoEstudio? ObtieneContacto(string hl7data)
    {
        throw new NotImplementedException();
    }
}
