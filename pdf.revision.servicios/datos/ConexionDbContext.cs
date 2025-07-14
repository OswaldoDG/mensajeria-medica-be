using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace pdf.revision.servicios.datos
{
    public class ConexionDbContext : IDesignTimeDbContextFactory<DbContextPdf>
    {
        public DbContextPdf CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContextPdf>();

            var connectionString = "server=localhost;port=3306;database=pdf_revision;user=root;password=Pa$$w0rd";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new DbContextPdf(optionsBuilder.Options);
        }
    }
}
