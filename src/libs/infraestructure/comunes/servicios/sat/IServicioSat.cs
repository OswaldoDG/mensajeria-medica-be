namespace comunes.servicios.crm.sat;

public interface IServicioSat
{
    /// <summary>
    /// OBtiene el listado de las formas de pago.
    /// </summary>
    /// <returns>Lista de formas de pago.</returns>
    List<FormaPago> FormasPago();

    /// <summary>
    /// OBtiene el listado de las métodos de pago.
    /// </summary>
    /// <returns>Lista de mpetodos de pago.</returns>
    List<MetodoPago> MetodosPago();

    /// <summary>
    /// OBtiene el listado de regimenes fiscales.
    /// </summary>
    /// <returns>Lista de regimenes fiscales.</returns>
    List<RegimenFiscal> RegimenesFiscales();

    /// <summary>
    /// OBtiene el listado de usos de Cfdi.
    /// </summary>
    /// <returns>Lista de usos de Cfdi.</returns>
    List<UsoCfdi> UsosCfdi();

    /// <summary>
    /// VErifica una forma de pago desde la clave.
    /// </summary>
    /// <param name="clave">Clave a validar</param>
    /// <returns>True si el elemento existe en el catálogo.</returns>
    bool FormaPagoValida(string clave);

    /// <summary>
    /// VErifica un método de pago desde la clave.
    /// </summary>
    /// <param name="clave">Clave a validar</param>
    /// <returns>True si el elemento existe en el catálogo.</returns>
    bool MetodoPagoValido(string clave);

    /// <summary>
    /// VErifica un régimen fiscal desde la clave.
    /// </summary>
    /// <param name="clave">Clave a validar</param>
    /// <returns>True si el elemento existe en el catálogo.</returns>
    bool RegimenFiscalValido(string clave);

    /// <summary>
    /// VErifica uso de Cfdi desde la clave.
    /// </summary>
    /// <param name="clave">Clave a validar</param>
    /// <returns>True si el elemento existe en el catálogo.</returns>
    bool UsoCfdiValido(string clave);

    /// <summary>
    /// Devuelve la clave de una forma de pago desde le nombre.
    /// </summary>
    /// <param name="nombre">Nombre a verificar.</param>
    /// <returns>Clave o nulo si el nombre no es encontrado.</returns>
    string? FormaPagoDeNombre(string nombre);

    /// <summary>
    /// Devuelve la clave de un método de pago desde le nombre.
    /// </summary>
    /// <param name="nombre">Nombre a verificar.</param>
    /// <returns>Clave o nulo si el nombre no es encontrado.</returns>
    string? MetodoPagoDeNombre(string nombre);

    /// <summary>
    /// Devuelve la clave de un régimen fiscal desde le nombre.
    /// </summary>
    /// <param name="nombre">Nombre a verificar.</param>
    /// <returns>Clave o nulo si el nombre no es encontrado.</returns>
    string? RegimenFiscalDeNombre(string nombre);

    /// <summary>
    /// Devuelve la clave de un uso de Cfdi desde le nombre.
    /// </summary>
    /// <param name="nombre">Nombre a verificar.</param>
    /// <returns>Clave o nulo si el nombre no es encontrado.</returns>
    string? UsoCfdiDeNombre(string nombre);

    /// <summary>
    /// Valida si una calve de uso ´puede ser utilizada por un tipo de régimen.
    /// </summary>
    /// <param name="claveRegimen">Clave del régimen.</param>
    /// <param name="claveUso">Clave de uso del Cfdi.</param>
    /// <returns>True si es valido el uso.</returns>
    bool UsoValidoCfdi(string claveRegimen, string claveUso);

    /// <summary>
    /// Devuelve la lista de usos por regimen fiscal. 
    /// </summary>
    /// <param name="clave">Clave del regimen.</param>
    /// <returns>Lista de usos del regimen.</returns>
    List<UsoCfdi> UsosPorRegimenFiscal(string clave);

    /// <summary>
    /// DEtermina si un RFC es válido.
    /// </summary>
    /// <param name="rfc">Rfc a validar.</param>
    /// <returns>True si es válido.</returns>
    bool RfcValido(string rfc);
}
