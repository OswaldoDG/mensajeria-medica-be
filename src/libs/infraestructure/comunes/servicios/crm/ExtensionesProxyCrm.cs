namespace comunes.servicios.crm;

/// <summary>
/// Extensiones del CRM.
/// </summary>
public static class ExtensionesProxyCrm
{
    public static string AString(this DireccionFiscal direccion)
    {
        return $"{direccion.NombreVialidad} {direccion.NumExterior} {direccion.NumInterior} {direccion.Colonia} {direccion.Municipio} {direccion.EntidadFederativa} {direccion.CodigoPostal}";
    }
}
