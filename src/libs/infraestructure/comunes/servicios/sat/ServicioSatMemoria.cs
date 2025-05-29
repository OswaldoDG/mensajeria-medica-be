using System.Text.RegularExpressions;

namespace comunes.servicios.crm.sat;

/// <summary>
/// Servicios relacionados con el SAT versión en memoria.
/// </summary>
public class ServicioSatMemoria : IServicioSat
{
    private readonly List<FormaPago> formasPago = ExtensionesSat.ObtieneFormasDePago();
    private readonly List<MetodoPago> metodosPago = ExtensionesSat.ObtieneMetodosDePago();
    private readonly List<RegimenFiscal> regimenesFiscales = ExtensionesSat.ObtieneRegimenesFiscales();
    private readonly List<UsoCfdi> usosCdfdi = ExtensionesSat.ObtieneUsosCfdi();
    private List<UsoRegimenFiscal> usosRegimen = [];

    public string? FormaPagoDeNombre(string nombre)
    {
        return formasPago.FirstOrDefault(x => x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase))?.Clave;
    }

    public bool FormaPagoValida(string clave)
    {
        return formasPago.Any(x => x.Clave.Equals(clave, StringComparison.InvariantCultureIgnoreCase));
    }

    public List<FormaPago> FormasPago()
    {
        return formasPago;
    }

    public string? MetodoPagoDeNombre(string nombre)
    {
        return metodosPago.FirstOrDefault(x => x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase))?.Clave;
    }

    public bool MetodoPagoValido(string clave)
    {
        return metodosPago.Any(x => x.Clave.Equals(clave, StringComparison.InvariantCultureIgnoreCase));
    }

    public List<MetodoPago> MetodosPago()
    {
        return metodosPago;
    }

    public List<RegimenFiscal> RegimenesFiscales()
    {
        return regimenesFiscales;
    }

    public string? RegimenFiscalDeNombre(string nombre)
    {
        nombre = nombre.RemoveSpahished();
        foreach (var regimen in regimenesFiscales)
        {
            if (nombre.EndsWith(regimen.Nombre.RemoveSpahished(), StringComparison.InvariantCultureIgnoreCase))
            {
                return regimen.Clave;
            }
        }

        return null;
    }

    public bool RegimenFiscalValido(string clave)
    {
        return regimenesFiscales.Any(x => x.Clave.Equals(clave, StringComparison.InvariantCultureIgnoreCase));
    }

    public bool RfcValido(string rfc)
    {
        string patronRfc = "^([A-ZÑ&]{3,4})\\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01])[A-Z\\d]{2}[A\\d]$";
        return Regex.IsMatch(rfc, patronRfc);
    }

    public string? UsoCfdiDeNombre(string nombre)
    {
        return usosCdfdi.FirstOrDefault(x => x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase))?.Clave;
    }

    public bool UsoCfdiValido(string clave)
    {
        return usosCdfdi.Any(x => x.Clave.Equals(clave, StringComparison.InvariantCultureIgnoreCase));
    }

    public List<UsoCfdi> UsosCfdi()
    {
        return usosCdfdi;
    }

    public List<UsoCfdi> UsosPorRegimenFiscal(string clave)
    {
        if (usosRegimen.Count == 0)
        {
            usosRegimen = CalculaUsosPorRegimen();
        }

        var usos = usosRegimen.FirstOrDefault(x => x.ClaveRegimen.EndsWith(clave, StringComparison.InvariantCultureIgnoreCase));
        return usos != null ? usos.Usos : [];
    }

    public bool UsoValidoCfdi(string claveRegimen, string claveUso)
    {
        var usoRegimen = usosRegimen.FirstOrDefault(x => x.ClaveRegimen.Equals(claveRegimen, StringComparison.InvariantCultureIgnoreCase));
        if (usosRegimen != null)
        {
           return usoRegimen!.Usos.Any(u => u.Clave.Equals(claveUso, StringComparison.InvariantCultureIgnoreCase));
        }

        return false;
    }

    private List<UsoRegimenFiscal> CalculaUsosPorRegimen()
    {
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var regimen in regimenesFiscales)
        {
            UsoRegimenFiscal uso = new ()
            {
                ClaveRegimen = regimen.Clave,
                Usos = usosCdfdi.Where(u => u.RegimenReceptor.Contains(regimen.Clave, StringComparison.InvariantCultureIgnoreCase)).ToList()!
            };
            usosRegimen.Add(uso);
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

        return usosRegimen;
    }
}
