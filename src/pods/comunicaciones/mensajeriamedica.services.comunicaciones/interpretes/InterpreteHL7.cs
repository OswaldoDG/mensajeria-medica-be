using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.comunicaciones.hl7;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace mensajeriamedica.services.comunicaciones.interpretes;

/// <summary>
/// Interprete de mensajes HL7 para extraer información de contacto del estudio.
/// </summary>
[ExcludeFromCodeCoverage]
public class InterpreteHL7(ILogger<InterpreteHL7> logger, IConfiguration config) : IInterpreteHL7
{
    public RespuestaContacto Parse(string mensaje)
    {
        var contacto = new ContactoEstudio();
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(mensaje))
            return new RespuestaContacto { Contacto = contacto, Mensaje = "Mensaje vacío" };

        var lineas = mensaje.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (lineas.Length <= 1 && mensaje.Contains("PID|"))
        {
            errores.Add("Advertencia: Se detectó solo 1 línea. Verifique los separadores de fila.");
        }

        foreach (var linea in lineas)
        {
            if (linea.StartsWith("PID|"))
            {
                var partes = linea.Split('|');

                if (partes.Length > 5 && !string.IsNullOrWhiteSpace(partes[5]))
                {
                    var n = partes[5].Split('^');
                    var apellido = n.Length > 0 ? n[0] : "";
                    var nombre = n.Length > 1 ? n[1] : "";
                    var segundo = n.Length > 2 ? n[2] : "";

                    contacto.NombreContacto = string.Join(" ", new[] { nombre, segundo, apellido }.Where(x => !string.IsNullOrWhiteSpace(x)));
                }

                if (partes.Length > 11 && !string.IsNullOrWhiteSpace(partes[11]))
                {
                    var direccion = partes[11].Split('^');
                    if (direccion.Length > 5)
                    {
                        contacto.Pais = direccion[5];
                    }
                }

                if (partes.Length > 13 && !string.IsNullOrWhiteSpace(partes[13]))
                {
                    var t = partes[13].Split('^');
                    contacto.Telefono = t.Length > 6 ? t[6] : null;
                }
            }
            else if (linea.StartsWith("PV1|"))
            {
                var partes = linea.Split('|');

                if (partes.Length > 3 && !string.IsNullOrWhiteSpace(partes[3]))
                {
                    var h = partes[3].Split('^');
                    contacto.SucursalId = h.Length > 3 ? h[3] : null;
                }
            }
            else if (linea.StartsWith("OBR|"))
            {
                var partes = linea.Split('|');

                if (partes.Length > 21 && !string.IsNullOrWhiteSpace(partes[21]))
                {
                    var u = partes[21].Split('^');
                    contacto.Url = u.Length > 0 ? u[0] : null;
                }
            }
        }

        bool tieneNombre = !string.IsNullOrWhiteSpace(contacto.NombreContacto);
        bool tieneTelefono = !string.IsNullOrWhiteSpace(contacto.Telefono);
        bool tieneUrl = !string.IsNullOrWhiteSpace(contacto.Url);

        if (!tieneNombre) errores.Add("Nombre no obtenido");
        if (!tieneTelefono) errores.Add("Teléfono no obtenido");
        if (!tieneUrl) errores.Add("URL no obtenida");

        contacto.DatosValidos = tieneNombre && tieneTelefono && tieneUrl;

        return new RespuestaContacto
        {
            Contacto = contacto,
            Mensaje = errores.Count > 0 ? string.Join("; ", errores) : "OK"
        };
    }
}