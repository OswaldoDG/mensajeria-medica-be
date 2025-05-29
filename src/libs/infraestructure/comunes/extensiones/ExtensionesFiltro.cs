using System.Globalization;
using System.Reflection;
using comunes.busqueda;

namespace comunes.extensiones;

public enum TipoDato
{
    Desconocido = 0,
    Texto = 1,
    Numero = 2,
    Booleano = 3,
    Fecha = 4,
    Enumeracion = 5
}

public static class ExtensionesFiltro
{
    public static string OrdenarBusqueda(this Busqueda busqueda)
    {
        if (!string.IsNullOrEmpty(busqueda.OrdenarPropiedad))
        {
            string ordenConsulta = "ORDER BY ";
            ordenConsulta += busqueda.OrdenarPropiedad + " ";
            if (busqueda.OrdernarDesc)
            {
                ordenConsulta += "DESC";
            }
            else
            {
                ordenConsulta += "ASC";
            }

            return ordenConsulta;
        }

        return string.Empty;
    }

    public static bool PaginadoValido(this Busqueda busqueda)
    {
        if (busqueda.Paginado != null)
        {
            if (busqueda.Paginado.Pagina < 0 || busqueda.Paginado.TamanoPagina <= 0)
            {
                return false;
            }
        }

        return true;
    }

    public static string PaginarBusqueda(this Busqueda busqueda)
    {
        string paginado = busqueda.Paginado != null ? $" LIMIT {busqueda.Paginado!.TamanoPagina} OFFSET {(busqueda.Paginado.Pagina - 1) * busqueda.Paginado.TamanoPagina}" : string.Empty;
        return paginado;
    }

    public static TipoDato TipoParaPropiedad<T>(string nombre)
    {
        var t = typeof(T);

        PropertyInfo[] propiedades = t.GetProperties ();

        var propiedad = propiedades.FirstOrDefault(e => e.Name.Equals(nombre, StringComparison.InvariantCultureIgnoreCase));

        if (propiedad?.PropertyType is null) return TipoDato.Desconocido;

        Type tipo = propiedad.PropertyType;
        tipo = Nullable.GetUnderlyingType(tipo) ?? tipo;

        if (tipo.IsEnum)
            return TipoDato.Enumeracion;

        if (tipo == typeof(Guid))
        {
            return TipoDato.Texto;
        }

#pragma warning disable CS8524 // La expresión switch no controla algunos valores de su tipo de entrada (no es exhaustiva) que requieran un valor de enumeración sin nombre.
        return Type.GetTypeCode(tipo) switch
        {
            TypeCode.Boolean => TipoDato.Booleano,
            TypeCode.String => TipoDato.Texto,
            TypeCode.Int32 => TipoDato.Numero,
            TypeCode.Int64 => TipoDato.Numero,
            TypeCode.Decimal => TipoDato.Numero,
            TypeCode.DateTime => TipoDato.Fecha,
            TypeCode.Empty => throw new NotImplementedException(),
            TypeCode.Object => throw new NotImplementedException(),
            TypeCode.DBNull => throw new NotImplementedException(),
            TypeCode.Char => throw new NotImplementedException(),
            TypeCode.SByte => throw new NotImplementedException(),
            TypeCode.Byte => throw new NotImplementedException(),
            TypeCode.Int16 => TipoDato.Numero,
            TypeCode.UInt16 => TipoDato.Numero,
            TypeCode.UInt32 => TipoDato.Numero,
            TypeCode.UInt64 => TipoDato.Numero,
            TypeCode.Single => TipoDato.Numero,
            TypeCode.Double => TipoDato.Numero
        };
#pragma warning restore CS8524 // La expresión switch no controla algunos valores de su tipo de entrada (no es exhaustiva) que requieran un valor de enumeración sin nombre.
    }

    public static Type? TipoParaPropiedadEnum<T>(string nombre)
    {
        var t = typeof(T);

        PropertyInfo[] propiedades = t.GetProperties();

        var propiedad = propiedades.FirstOrDefault(e => e.Name.Equals( nombre, StringComparison.InvariantCultureIgnoreCase));

        if (propiedad?.PropertyType is null) return null;

        return propiedad.PropertyType;
    }

    public static int? ValorIntPropiedadEnum(Type t, string valor)
    {
        if (Enum.TryParse(t, valor, true, out object? tipo) && tipo != null)
        {
            return tipo.GetHashCode();
        }

        return null;
    }

    public static string SQL<T>(this List<Filtro>? filtros)
        where T : class
    {
        if (filtros != null)
        {
            List<string> condiciones = [];

            filtros.ForEach(filtro =>
            {
                TipoDato tipo = TipoParaPropiedad<T>(filtro.Propiedad);
                if (tipo != TipoDato.Desconocido)
                {
                    string condicion = filtro.CondidionSQL<T>(tipo);
                    if (!string.IsNullOrEmpty(condicion))
                    {
                        condiciones.Add(condicion);
                    }
                }
            });

            if (condiciones.Count > 0)
            {
                if (condiciones.Count == 1)
                {
                    return condiciones[0];
                }
                else
                {
                    return string.Join(" AND ", condiciones);
                }
            }
        }

        return "";
    }

    public static string SQL<T>(this List<Filtro>? filtros, string listaCamposCsv, string prefijo = "")
    where T : class
    {

        List<string> campos = listaCamposCsv.Split(',').ToList();

        if (filtros != null)
        {
            List<string> condiciones = [];

            filtros.ForEach(filtro =>
            {
                if (string.IsNullOrEmpty(listaCamposCsv) || campos.Contains(filtro.Propiedad, StringComparer.InvariantCultureIgnoreCase))
                {
                    TipoDato tipo = TipoParaPropiedad<T>(filtro.Propiedad);
                    if (tipo != TipoDato.Desconocido)
                    {
                        string condicion = filtro.CondidionSQL<T>(tipo, prefijo);
                        if (!string.IsNullOrEmpty(condicion))
                        {
                            condiciones.Add(condicion);
                        }
                    }
                }
            });

            if (condiciones.Count > 0)
            {
                if (condiciones.Count == 1)
                {
                    return condiciones[0];
                }
                else
                {
                    return string.Join(" AND ", condiciones);
                }
            }
        }

        return "";
    }

    public static string SQL<T1, T2>(this List<Filtro>? filtros)
        where T1 : class
        where T2 : class
    {
        if (filtros != null)
        {
            List<string> condiciones = [];

            filtros.ForEach(filtro =>
            {
                TipoDato tipo;
                string condicion = string.Empty;
                string prefijo = string.Empty;

                tipo = TipoParaPropiedad<T1>(filtro.Propiedad);
                if (tipo != TipoDato.Desconocido)
                {
                    prefijo = "L.";
                    condicion = filtro.CondidionSQL<T1>(tipo);
                }
                else
                {
                    tipo = TipoParaPropiedad<T2>(filtro.Propiedad);
                    if (tipo != TipoDato.Desconocido)
                    {
                        prefijo = "P.";
                        condicion = filtro.CondidionSQL<T2>(tipo);
                    }
                }

                if (!string.IsNullOrEmpty(condicion))
                {
                    string condicionConPrefijo = condicion.Insert(0, prefijo);
                    condiciones.Add(condicionConPrefijo);
                }
            });

            if (condiciones.Count > 0)
            {
                return string.Join(" AND ", condiciones);
            }
        }

        return "";
    }

    private static string Prefijo(this string prefijo)
    {
        return string.IsNullOrEmpty (prefijo) ? string.Empty : $"{prefijo}.";
    }

    public static string CondidionSQL<T>(this Filtro filtro, TipoDato tipo, string prefijo = "")
    {
        string condicion = string.Empty;
        switch (tipo)
        {

            case TipoDato.Fecha:
                condicion = $"{(filtro.Negacion.HasValue && filtro.Negacion.Value ? "NOT" : "")} {filtro.CondicionFechaSQL(prefijo)}";
                break;
            case TipoDato.Numero:
                condicion = $"{(filtro.Negacion.HasValue && filtro.Negacion.Value ? "NOT" : "")} {filtro.CondicionNumeroSQL(prefijo)}";
                break;
            case TipoDato.Texto:
                condicion = filtro.CondicionTextoSQL(prefijo);
                break;
            case TipoDato.Enumeracion:
                condicion = filtro.CondicionEnumeracion<T>(prefijo);
                break;
        }

        return condicion;
    }

    public static string CondicionTextoSQL(this Filtro filtro, string prefijo = "")
    {
        foreach (var valor in filtro!.Valores!)
        {
            if (filtro.Valores.Count == 0 || string.IsNullOrEmpty(valor))
            {
                return string.Empty;
            }
        }

        return filtro.Operador switch
        {
            Operador.Igual => $"{prefijo.Prefijo()}{filtro.Propiedad} {(filtro.Negacion.HasValue && filtro.Negacion.Value ? "<>" : "=")} '{filtro.Valores[0]}' ",
            Operador.Comienza => $"{prefijo.Prefijo()}{filtro.Propiedad} {(filtro.Negacion.HasValue && filtro.Negacion.Value ? "NOT" : "")} LIKE '{filtro.Valores[0]}%' ",
            Operador.Termina => $"{prefijo.Prefijo()}{filtro.Propiedad} {(filtro.Negacion.HasValue && filtro.Negacion.Value ? "NOT" : "")} LIKE '%{filtro.Valores[0]}' ",
            Operador.Contiene => $"{prefijo.Prefijo()}{filtro.Propiedad} {(filtro.Negacion.HasValue && filtro.Negacion.Value ? "NOT" : "")} LIKE '%{filtro.Valores[0]}%'",
            _ => string.Empty,
        };
    }


    public static string CondicionEnumeracion<T>(this Filtro filtro, string prefijo = "")
    {

        Type? tipoPropiedad = TipoParaPropiedadEnum<T>(filtro.Propiedad);
        if (tipoPropiedad != null)
        {
            int? valorPropiedad = ValorIntPropiedadEnum(tipoPropiedad, filtro.Valores![0]);
            if (valorPropiedad != null)
            {
                return $"{prefijo.Prefijo()}{filtro.Propiedad} {(filtro.Negacion.HasValue && filtro.Negacion.Value ? "<>" : "=")} {valorPropiedad}";
            }

        }

        return string.Empty;
    }

    public static string CondicionNumeroSQL(this Filtro filtro, string prefijo = "")
    {
        List<decimal> numeros = new ();
        foreach (var valor in filtro!.Valores!)
        {
            if (decimal.TryParse(valor, System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal n))
            {
                numeros.Add(n);
            }
            else
            {
                return string.Empty;
            }
        }

        if (numeros.Count == 0 || (filtro.Operador == Operador.Entre && numeros.Count != 2 && numeros[0] == numeros[1]))
        {
            return string.Empty;
        }

        switch (filtro.Operador)
        {
            case Operador.Mayor:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} > {numeros[0]}";
            case Operador.Menor:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} < {numeros[0]} ";
            case Operador.Igual:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} = {numeros[0]} ";
            case Operador.MayorIgual:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} >= {numeros[0]} ";
            case Operador.MenorIgual:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} <= {numeros[0]} ";
            case Operador.Entre:
                return numeros[0] < numeros[1]
                    ? $"{prefijo.Prefijo()}{filtro.Propiedad} BETWEEN {numeros[0]} AND {numeros[1]}"
                    : $"{prefijo.Prefijo()}{filtro.Propiedad} BETWEEN {numeros[1]} AND {numeros[0]}";
        }

        return string.Empty;
    }

    public static string CondicionFechaSQL(this Filtro filtro, string prefijo = "")
    {
        List<string> fechas = new ();

        foreach (var valor in filtro!.Valores!)
        {
#pragma warning disable S6580 // Use a format provider when parsing date and time, LA FECHA DEBE VENIR EN FORMATO ISO
            if (DateTime.TryParse(valor, out _))
            {
                fechas.Add(valor.TrimEnd('Z'));
            }
            else
            {
                return string.Empty;
            }
#pragma warning restore S6580 // Use a format provider when parsing date and time
        }

        if (fechas.Count == 0 || (filtro.Operador == Operador.Entre && fechas.Count != 2 && fechas[0] == fechas[1]))
        {
            return string.Empty;
        }

        switch (filtro.Operador)
        {
            case Operador.Mayor:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} > '{fechas[0]}'";
            case Operador.Menor:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} < '{fechas[0]}' ";
            case Operador.Igual:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} = '{fechas[0]}' ";
            case Operador.MayorIgual:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} >= '{fechas[0]}' ";
            case Operador.MenorIgual:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} <= '{fechas[0]}'";
            case Operador.Entre:
                return $"{prefijo.Prefijo()}{filtro.Propiedad} BETWEEN '{fechas[0]}' AND '{fechas[1]}'";
        }

        return string.Empty;
    }
}
