using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using comunes.busqueda;
using comunes.respuestas;
using mensajeriamedica.model.identity.registro;

namespace contabee.model.identity.usuarios;

/// <summary>
/// Extensiones de usuario.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ExtensionesUsuario
{
    private const string CAMPOSVALIDOSFILTROUSUARIO = ",PhoneNumber,EmailConfirmed,Email,UserName,Id,CuentaFiscalId,Estado,FechaRegistro,FechaActivacion,TipoCuenta,Nombre,";


    public static bool RolesValidos(this List<string> rolesNuevos, List<string> rolesValidos)
    {
        var interseccion = rolesNuevos.Select(r => r.ToLower()).Intersect(rolesValidos).ToList();
        return interseccion.Count == rolesNuevos.Count;
    }

    public static string StringRolesActualizados(List<string> actuales, List<string> nuevos, bool eliminarAnteriores)
    {
        string roles = string.Empty;
        if (eliminarAnteriores)
        {
            roles = string.Join(",", roles);
        }
        else
        {
            foreach (var nuevo in nuevos)
            {
                if (!actuales.Any(x => x.Equals(nuevo, StringComparison.OrdinalIgnoreCase)))
                {
                    actuales.Add(nuevo);
                }
            }

            roles = string.Join(",", roles);
        }

        return roles;
    }

    public static ErrorProceso? ValidaContextoBusqueda(this Busqueda busqueda, Guid? cuentaFiscalId = null)
    {
        //if (busqueda == null) return null;

        //Filtro? f = busqueda.ObtieneFiltroBusqueda("TipoCuenta");
        //if (cuentaFiscalId != null)
        //{
        //    if (f == null && f?.Valores?.FirstOrDefault() == null)
        //    {
        //        return ErroresUsuario.DatoFiltroRequerido("TipoCuenta");
        //    }

        //    if (!Enum.TryParse(f!.Valores!.FirstOrDefault()!, out TipoCuenta tipoCuenta))
        //    {
        //        return ErroresUsuario.DatoFiltroValorNoValido("TipoCuenta", f!.Valores!.FirstOrDefault()!);
        //    }

        //    if (cuentaFiscalId != null &&
        //        (tipoCuenta != TipoCuenta.EmpleadoCliente && tipoCuenta != TipoCuenta.LoginLessCliente))
        //    {
        //        return ErroresUsuario.DatoFiltroTipoUsuario();
        //    }
        //}

        //f = busqueda.ObtieneFiltroBusqueda("Estado");
        //if (f != null && !Enum.TryParse(f!.Valores!.FirstOrDefault()!, out EstadoCuenta _))
        //{
        //    return ErroresUsuario.DatoFiltroValorNoValido("Estado", f!.Valores!.FirstOrDefault()!);
        //}

        return null;
    }

    public static Busqueda EliminaFiltrosUsuarioNoValidos(this Busqueda busqueda, Guid? cuentaFiscalId = null)
    {
        busqueda.Filtros ??= [];

        if (cuentaFiscalId.HasValue)
        {
            busqueda = busqueda.ReemplazaValorFiltro("CuentaFiscalId", [cuentaFiscalId!.Value.ToString()], Operador.Igual);
        }

        List<Filtro> filtros = [];
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var filtro in busqueda.Filtros)
        {
            if (CAMPOSVALIDOSFILTROUSUARIO.Contains($",{filtro.Propiedad},", StringComparison.InvariantCultureIgnoreCase))
            {
                filtros.Add(filtro);
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

        busqueda.Filtros = filtros;

        return busqueda;
    }

    public static Busqueda ReemplazaValorFiltro(this Busqueda busqueda, string propiedad, List<string> valores, Operador operador = Operador.Igual)
    {
        var f = busqueda.ObtieneFiltroBusqueda(propiedad);
        if (f != null)
        {
            busqueda.Filtros.Remove(f);
        }

        f = new Filtro() { Propiedad = propiedad, Valores = valores, Operador = operador };
        busqueda.Filtros.Add(f);
        return busqueda;
    }

    public static Filtro? ObtieneFiltroBusqueda(this Busqueda busqueda, string propiedad)
    {
        var f = busqueda.Filtros.FirstOrDefault(x => x.Propiedad.Equals(propiedad, StringComparison.InvariantCultureIgnoreCase));
        return f;
    }

    public static ResultadoPaginado<CuentaUsuario> ToPaginadoCuentaUsuario(this ResultadoPaginado<ApplicationUser> paginado)
    {
        ResultadoPaginado<CuentaUsuario> resultado = new ()
        {
            Contar = paginado.Contar,
            Filtros = paginado.Filtros,
            OrdenarPropiedad = paginado.OrdenarPropiedad,
            OrdernarDesc = paginado.OrdernarDesc,
            Total = paginado.Total,
            Paginado = paginado.Paginado,
            Elementos = [],
        };

        foreach (var usuario in paginado.Elementos)
        {
            resultado.Elementos.Add(usuario.ToCuentaUsuario()!);
        }

        return resultado;
    }

    public static CuentaUsuario? ToCuentaUsuario(this ApplicationUser user)
    {
        if (user == null) return null;
        CuentaUsuario cuenta = new ()
        {
            Email = user.Email,
            CreadorId = user.CreadorId,
            CuentaFiscalId = user.CuentaFiscalId,
            Estado = user.Estado,
            FechaActivacion = user.FechaActivacion,
            FechaCaducidadTokenLoginless = null,
            FechaRegistro = user.FechaRegistro,
            Id = Guid.Parse(user.Id),
            Nombre = user.Nombre,
            TipoCuenta = user.TipoCuenta
        };

        return cuenta;
    }

    public static ValidadorFuncional<CreaUsuario> ValidaEmail(this ValidadorFuncional<CreaUsuario> datos)
    {
        if (datos.Instancia == null)
        {
            datos.Error = ErroresUsuario.DatosNulos(nameof(CreaUsuario));
        }
        else
        {
            if (string.IsNullOrEmpty(datos.Instancia.Email))
            {
                datos.Error = ErroresUsuario.EmailRequerido();
            }
        }

        return datos;
    }

    public static ValidadorFuncional<CreaUsuario> ValidaContrasena(this ValidadorFuncional<CreaUsuario> datos)
    {
        if (datos.Instancia == null)
        {
            datos.Error = ErroresUsuario.DatosNulos(nameof(CreaUsuario));
        }
        else
        {
            if (string.IsNullOrEmpty(datos.Instancia.Password))
            {
                datos.Error = ErroresUsuario.ContrasenaRequerida();
            }
        }

        return datos;
    }

    public static ValidadorFuncional<CreaUsuario> ValidaIdCuentaFiscal(this ValidadorFuncional<CreaUsuario> datos)
    {
        if (datos.Instancia == null)
        {
            datos.Error = ErroresUsuario.DatosNulos(nameof(CreaUsuario));
        }
        else
        {
            if (!datos.Instancia.CuentaFiscalId.HasValue)
            {
                datos.Error = ErroresUsuario.CuentaFiscalRequerida();
            }
        }

        return datos;
    }

    public static RespuestaPayload<ApplicationUser> CreaUsuarioApp(this CreaUsuario datos, Guid usuarioCreadorId, Guid? cuentaFiscalId)
    {
        RespuestaPayload<ApplicationUser> respuesta = new ();
        ValidadorFuncional<CreaUsuario> validador = new ValidadorFuncional<CreaUsuario>(datos);

        ApplicationUser user = new ()
        {
            Nombre = datos.Nombre,
            Email = datos.Email,
            UserName = datos.Email,
            PhoneNumber = datos.Telefono,
            CreadorId = usuarioCreadorId,
            CuentaFiscalId = cuentaFiscalId,
            FechaActivacion = DateTime.UtcNow,
            EmailConfirmed = true,
            Estado = EstadoCuenta.Activo,
            TipoCuenta = datos.TipoCuenta
        };

        switch (datos.TipoCuenta)
        {
            case TipoCuenta.LoginLessCliente:
                user.UserName = $"loginless-{Guid.NewGuid().ToString().Replace("-", "")}";
                respuesta.Error = validador.ValidaIdCuentaFiscal().Error;
                break;

            case TipoCuenta.Cliente:
                respuesta.Error = validador.ValidaEmail().ValidaContrasena().Error;
                break;

            case TipoCuenta.Empleado:
                respuesta.Error = validador.ValidaEmail().ValidaContrasena().Error;

                break;

            case TipoCuenta.EmpleadoCliente:
                respuesta.Error = validador.ValidaEmail().ValidaContrasena().ValidaIdCuentaFiscal().ValidaContrasena().Error;
                break;
        }

        respuesta.Payload = respuesta.Error == null ? user : null;
        return respuesta;
    }
}
