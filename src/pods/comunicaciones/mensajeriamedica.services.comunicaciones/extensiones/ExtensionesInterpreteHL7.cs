using System.Text;
using mensajeriamedica.model.comunicaciones.hl7;

namespace mensajeriamedica.services.comunicaciones.extensiones;
public static class ExtensionesInterpreteHL7
{
    public static ContactoEstudio ProcesarContenidoHL7(this string contenidoArchivo,
        HashSet<string> marcadoresRequeridos,
        Dictionary<string, List<CampoConfig>> mapeoCampos)
    {
        string[] lineas = contenidoArchivo.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        if (!lineas.ValidarPresenciaMarcadores(marcadoresRequeridos))
        {
            return new ContactoEstudio() { DatosValidos = false };
        }

        ContactoEstudio contacto = new ContactoEstudio();
        string marcadorActual = null;
        StringBuilder parrafoActual = new StringBuilder();

        foreach (string linea in lineas)
        {
            string posibleMarcador = linea.Length >= 3 ? linea[..3] : "";

            if (marcadoresRequeridos.Contains(posibleMarcador))
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

    public static bool ValidarPresenciaMarcadores(
        this IEnumerable<string> lineas,
        HashSet<string> marcadoresRequeridos)
    {
        var marcadoresEncontrados = new HashSet<string>();
        var faltantes = new HashSet<string>(marcadoresRequeridos);

        foreach (string linea in lineas)
        {
            if (linea.Length < 3) continue;

            string posibleMarcador = linea[..3];
            if (marcadoresRequeridos.Contains(posibleMarcador))
            {
                marcadoresEncontrados.Add(posibleMarcador);
                faltantes.Remove(posibleMarcador);

                if (faltantes.Count == 0)
                {
                    return true;
                }
            }
        }

        return faltantes.Count == 0;
    }

    public static ContactoEstudio ProcesarSegmentoHL7(
        this ContactoEstudio contacto,
        string tipo,
        string contenido,
        Dictionary<string, List<CampoConfig>> mapeoCampos)
    {
        if (mapeoCampos.TryGetValue(tipo, out List<CampoConfig> configs))
        {
            foreach (var config in configs)
            {
                List<string> valores = contenido.ExtraerComponentesHL7(
                    config.PipeIndex,
                    config.Componentes
                );

                if (valores.Count > 0)
                {
                    var propiedad = typeof(ContactoEstudio).GetProperty(config.Propiedad);
                    if (propiedad != null)
                    {
                        string valorFinal = string.Join("", valores);
                        propiedad.SetValue(contacto, valorFinal);
                    }
                }
            }
        }
        return contacto;
    }

    public static List<string> ExtraerComponentesHL7(
        this string contenidoSegmento,
        int indicePipe,
        params int[] indicesComponentes)
    {
        var resultados = new List<string>();

        if (string.IsNullOrWhiteSpace(contenidoSegmento))
            return resultados;

        string primeraLinea = contenidoSegmento.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()!;

        if (primeraLinea == null) return resultados;

        string[] campos = primeraLinea.Split('|');

        if (campos.Length <= indicePipe)
            return resultados;

        string[] componentes = campos[indicePipe].Split('^');

        foreach (int index in indicesComponentes)
        {
            if (index >= 0 && index < componentes.Length)
                resultados.Add(componentes[index]);
            else
                resultados.Add(string.Empty);
        }

        return resultados;
    }

}