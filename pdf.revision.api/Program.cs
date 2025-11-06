using System.Reflection;
using comunes.extensiones;
using Microsoft.EntityFrameworkCore;
using pdf.revision.servicios;
using pdf.revision.servicios.datos;

namespace pdf.revision.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IWebHostEnvironment environment = builder.Environment;

            builder.CreaConfiguracionStandar(Assembly.GetExecutingAssembly());

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<DbContextPdf>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


            builder.Services.AddMemoryCache();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddTransient<IServicioPdf, ServicioPdf>();

            var app = builder.Build();


            Preprocess(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void Preprocess(WebApplication app)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContextPdf>();
                dbContext.Database.Migrate();

            }
            catch (Exception)
            {

            }
        }
    }
}
