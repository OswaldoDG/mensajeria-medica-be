using System.Diagnostics.CodeAnalysis;
using System.Text;
using mensajeriamedica.model.comunicaciones.hl7;
using mensajeriamedica.services.comunicaciones.extensiones;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.interpretes;

/// <summary>
/// Interprete de mensajes HL7 para extraer información de contacto del estudio.
/// </summary>
[ExcludeFromCodeCoverage]
public class InterpreteHL7(ILogger<InterpreteHL7> logger, IConfiguration config) : IInterpreteHL7
{
    public ContactoEstudio? ObtieneContacto(string hl7data)
    {
        HashSet<string> marcadores = config.GetSection("MarcadoresHL7")
            .Get<List<string>>()
            ?.ToHashSet() ?? new HashSet<string>();

        string[] lineas = hl7data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        Dictionary<string, List<CampoConfig>> mapeoCampos = config.GetSection("MapeoCampos")
            .Get<Dictionary<string, List<CampoConfig>>>() ?? new Dictionary<string, List<CampoConfig>>();

        if (!lineas.ValidarPresenciaMarcadores(marcadores))
        {
            return new ContactoEstudio() { DatosValidos = false };
        }

        ContactoEstudio contacto = new ContactoEstudio();
        string marcadorActual = null;
        StringBuilder parrafoActual = new StringBuilder();

        foreach (string linea in lineas)
        {
            string posibleMarcador = linea.Length >= 3 ? linea[..3] : "";

            if (marcadores.Contains(posibleMarcador))
            {
                if (marcadorActual != null && parrafoActual.Length > 0)
                {
                    contacto = contacto.ProcesarSegmentoHL7(marcadorActual, parrafoActual.ToString(), mapeoCampos);
                    parrafoActual.Clear();
                }

                marcadorActual = posibleMarcador;
            }

            if (marcadorActual != null)
            {
                if (parrafoActual.Length > 0)
                    parrafoActual.Append("\n");
                parrafoActual.Append(linea);
            }
        }

        if (marcadorActual != null && parrafoActual.Length > 0)
        {
            contacto = contacto.ProcesarSegmentoHL7(marcadorActual, parrafoActual.ToString(), mapeoCampos);
        }

        contacto.DatosValidos = true;
        return contacto;
    }
}