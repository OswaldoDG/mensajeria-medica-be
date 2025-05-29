using comunes.extensiones;
using Microsoft.EntityFrameworkCore;

namespace comunes.busqueda;

public class ServicioBusquedaSQL<T>(string tabla)
    where T : class
{

    public async Task<ResultadoPaginado<T>> Buscar(Busqueda busqueda, DbContext db)
    {
        db.Set<T>();
        ResultadoPaginado<T> pagina = new ResultadoPaginado<T>();

        if (!busqueda.PaginadoValido())
        {
            throw new Exception("Valores de paginado no válidos");
        } 
        else
        {
            string consultaBaseSQL = $"SELECT * FROM {tabla}";
            string condicionSQL = busqueda.Filtros.SQL<T>();

            if (!string.IsNullOrEmpty(condicionSQL))
            {
                condicionSQL = $" WHERE {condicionSQL} ";
            }

            string consultaQuery = $"{consultaBaseSQL} {condicionSQL} {busqueda.OrdenarBusqueda()} {busqueda.PaginarBusqueda()}";

            pagina.Elementos = await db.Set<T>().FromSqlRaw(consultaQuery).ToListAsync();

            pagina.Total = 0;

            if (busqueda.Contar)
            {
                var command = db.Database.GetDbConnection().CreateCommand();
                command.CommandText = $"{consultaBaseSQL.Replace("*", "count(*)")} {condicionSQL}";
                await db.Database.OpenConnectionAsync();
                var result = await command.ExecuteScalarAsync();

                pagina.Total = Convert.ToInt32(result);
                pagina.Paginado = busqueda.Paginado;
                pagina.Contar = busqueda.Contar;

                await command.DisposeAsync();
            }
        }

        return pagina;
    }
}
