using comunes.busqueda;
using comunes.extensiones;
using mensajeriamedica.model.identity.registro;
using mensajeriamedica.model.identity.tokenloginless;
using mensajeriamedica.model.identity.usuarios;
using mensajeriamedica.services.identity.dbcontext;
using Microsoft.EntityFrameworkCore;

namespace contabee.services.identity.usuarios;

public static class Extensiones
{

    private static string ObtieneConsultaBase()
    {
        return "SELECT U.*, null as FechaCaducidadTokenLoginless, R.RoleId as Roles, C.Roles as RolesCF FROM " + DbContextIdentity.TABLAUSUARIO + " U " +
               " left join " + DbContextIdentity.TABLAROLES + " R on U.id = R.UserId" +
               " left join " + DbContextIdentity.TABLAUSUARIOSROLCUENTAFISCAL + " C on U.id = C.UsuarioId";
    }

    private static string ObtieneConsultaBaseCount()
    {
        return "SELECT COUNT(U.Id) as Value from " + DbContextIdentity.TABLAUSUARIO + " U ";
    }

    public static ResultadoPaginado<CuentaUsuario> BusquedaConRol(this Busqueda busqueda, DbContextIdentity db)
    {
        ResultadoPaginado<CuentaUsuario> pagina = new ResultadoPaginado<CuentaUsuario>();

        var filtrosUsuario = busqueda.Filtros.SQL<ApplicationUser>("", "U");


        string consultaBaseSQL = ObtieneConsultaBase();
        string consultaContar = ObtieneConsultaBaseCount();
        string condicionesWhere = filtrosUsuario;

        if (!string.IsNullOrEmpty(condicionesWhere))
        {
            consultaBaseSQL += $" WHERE {condicionesWhere}";
            consultaContar += $" WHERE {condicionesWhere}";
        }

        string consultaQuery = $"{consultaBaseSQL} {busqueda.OrdenarBusqueda()} {busqueda.PaginarBusqueda()} ";

        var elementos = db.Database.SqlQueryRaw<CuentaUsuarioRoles>(consultaQuery).ToList();
        var unicos = elementos.DistinctBy(e => e.Id).ToList();
        List<CuentaUsuario> usuarios = [];
        foreach (var elemento in unicos)
        {
            var actual = elementos.First(u => u.Id == elemento.Id);
            CuentaUsuario usuario = new ()
            {
                CreadorId = actual.CreadorId,
                CuentaFiscalId = actual.CuentaFiscalId,
                Email = actual.Email,
                Estado = actual.Estado,
                FechaActivacion = actual.FechaActivacion,
                FechaCaducidadTokenLoginless = actual.FechaCaducidadTokenLoginless,
                FechaRegistro = actual.FechaRegistro,
                Id = actual.Id,
                Nombre = actual.Nombre,
                TipoCuenta = actual.TipoCuenta,
                Roles = [],
            };

            if (elemento.CuentaFiscalId != null)
            {
                var roles = db.RolesCuentaFiscal.Where(r => r.CuentaFiscalId == elemento.CuentaFiscalId && r.UsuarioId == elemento.Id)
                    .Select(r => r.Roles).FirstOrDefault();
                if (!string.IsNullOrEmpty(roles))
                {
                    usuario.Roles = roles.Split(',').Select(r => r.Trim()).ToList();
                }
            }
            else
            {
#pragma warning disable CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
                usuario.Roles = elementos.Where(u => u.Id == actual.Id && u.Roles != null).Select(r => r.Roles).ToList();
#pragma warning restore CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
            }
            usuarios.Add(usuario);
        }

        pagina.Elementos = usuarios;
        var totales = db.Database.SqlQueryRaw<int>(consultaContar!, new object[] { }).ToList();
        pagina.Total = totales.Count > 0 ? totales[0] : 0;

        return pagina;
    }

    public static ResultadoTokenLoginLess AResultadoTokenloginLess(Guid usuarioId, string token)
    {
        return new ResultadoTokenLoginLess() { Token = token, UsuarioId = usuarioId };
    }
}
