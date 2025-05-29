using System.Diagnostics.CodeAnalysis;

namespace comunes.busqueda;

/// <summary>
/// Configuracion de busqueda.
/// </summary>
[ExcludeFromCodeCoverage]
public class Busqueda
{
    /// <summary>
    /// Lista de filtros asociados a la busqueda.
    /// </summary>
    public List<Filtro> Filtros { get; set; } = [];

    /// <summary>
    /// M{etodo de ordenamiento.
    /// </summary>
    public bool OrdernarDesc { get; set; } = false;

    /// <summary>
    /// Propiedad utilziada para ordernar.
    /// </summary>
    public string? OrdenarPropiedad { get; set; }

    /// <summary>
    /// Datos para paginado.
    /// </summary>
    public Paginado? Paginado { get; set; }

    /// <summary>
    /// Especifica si la busqueda debe calcular el conteo de los elementos.
    /// </summary>
    public bool Contar { get; set; } = false;
}

/// <summary>
/// REsultados de una busqueda paginada generica.
/// </summary>
/// <typeparam name="T">Tipo de datos buscado.</typeparam>
[ExcludeFromCodeCoverage]
public class ResultadoPaginado<T> : Busqueda
{
    public List<T> Elementos { get; set; } = [];
    public long Total { get; set; }
}

/// <summary>
/// Define el paginado para una busqueda.
/// </summary>
public class Paginado
{
    /// <summary>
    /// Pagina de datos a obtener, base 0.
    /// </summary>
    public int Pagina { get; set; }

    /// <summary>
    /// Maximo numero de elemento a obtener en una pagina.
    /// </summary>
    public int TamanoPagina { get; set; }
}

/// <summary>
/// Elementro fltro para la busqueda.
/// </summary>
[ExcludeFromCodeCoverage]
public class Filtro
{
    /// <summary>
    /// Nombre de la propiedad para filtrar.
    /// </summary>
    required public string Propiedad { get; set; }

    /// <summary>
    /// Operador utilziado para la busqueda.
    /// </summary>
    public Operador Operador { get; set; }

    /// <summary>
    /// Especifica si el filtro se evalua como una negacion.
    /// </summary>
    public bool? Negacion { get; set; }

    /// <summary>
    /// Valores utilizador para el filtro, los valores se encuentran serialziados en formato Json.
    /// </summary>
    public List<string>? Valores { get; set; }
}

/// <summary>
/// Operadores validos para un filtro.
/// </summary>
public enum Operador
{
    /// <summary>
    /// No hay operador.
    /// </summary>
    None,

    /// <summary>
    /// Ifgualdad para cualquier tipo de dato.
    /// </summary>
    Igual,

    /// <summary>
    /// Mayor o igual para valores de rango.
    /// </summary>
    MayorIgual,

    /// <summary>
    /// Mayor para valores de rango.
    /// </summary>
    Mayor,

    /// <summary>
    /// Manor o igual para valores de rango.
    /// </summary>
    MenorIgual,

    /// <summary>
    /// Menor para valores de rango.
    /// </summary>
    Menor,

    /// <summary>
    /// VAlor entre 2 elmentos para valores de rango.
    /// </summary>
    Entre,

    /// <summary>
    /// Contiene para propiedades de tipo texto.
    /// </summary>
    Contiene,

    /// <summary>
    /// Comienza para propiedades de tipo texto.
    /// </summary>
    Comienza,

    /// <summary>
    /// Termina para propiedades de tipo texto.
    /// </summary>
    Termina,

    /// <summary>
    /// Propiedad = verdadero para valores booleanos.
    /// </summary>
    Verdadero,

    /// <summary>
    /// Propiedad = falso para valores booleanos.
    /// </summary>
    Falso
}