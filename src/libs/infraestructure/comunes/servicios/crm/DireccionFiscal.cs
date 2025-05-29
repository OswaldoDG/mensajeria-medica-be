using System.Diagnostics.CodeAnalysis;

namespace comunes.servicios.crm;

/// <summary>
/// Datos de ubicación fiscal.
/// </summary>
[ExcludeFromCodeCoverage]
public class DireccionFiscal
{

    /// <summary>
    /// Identificador único de la dirección fiscal.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Identificador unico de la cuenta fiscal.
    /// </summary>
    public Guid CuentaFiscalId { get; set; }

    /// <summary>
    /// Entidad federativa.
    /// </summary>
    required public string EntidadFederativa { get; set; }

    /// <summary>
    /// Municipio.
    /// </summary>
    required public string Municipio { get; set; }

    /// <summary>
    /// Colonia.
    /// </summary>
    required public string Colonia { get; set; }

    /// <summary>
    /// Tipo de vialidad.
    /// </summary>
    required public string TipoVialidad { get; set; }

    /// <summary>
    /// Nombre de vialidad.
    /// </summary>
    required public string NombreVialidad { get; set; }

    /// <summary>
    /// Número exterior.
    /// </summary>
    required public string NumExterior { get; set; }

    /// <summary>
    /// Número interior.
    /// </summary>
    required public string NumInterior { get; set; }

    /// <summary>
    /// Código postal.
    /// </summary>
    required public string CodigoPostal { get; set; }


    /// <summary>
    /// Email.
    /// </summary>
    required public string Email { get; set; }

    /// <summary>
    /// AL.
    /// </summary>
    required public string AL { get; set; }
}
