using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.comunicaciones.centroscostos;
using mensajeriamedica.model.comunicaciones.mensajes;
using mensajeriamedica.model.comunicaciones.servidores;
using mensajeriamedica.services.comunicaciones.configuracion;
using Microsoft.EntityFrameworkCore;

namespace mensajeriamedica.services.comunicaciones;

/// <summary>
/// Contexto de mensajeria.
/// </summary>
[ExcludeFromCodeCoverage]
public class DbContextMensajeria : DbContext
{
    public const string TABLA_SERVIDORES = "msj$servidores";
    public const string TABLA_MENSAJES = "msj$mensajes";
    public const string Tabla_ESTADISICASMENSAJES = "msj$estadisticasmensajes";
    public const string Tabla_CENTROCOSTOS = "msj$centrocostos";
    public const string Tabla_UNIDADCOSTOS = "msj$unidadcostos";
    public const string Tabla_USUARIOCENTROCOSTOS = "msj$usuariocentrocostos";
    public DbSet<Servidor> Servidores { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }
    public DbSet<EstadisticasMensajes> EstadisticasMensajes { get; set; }
    public DbSet<CentroCostos> CentroCostos { get; set; }
    public DbSet<UnidadCostos> UnidadCostos { get; set; }
    public DbSet<UsuarioCentroCostos> UsuarioCentrosCostos { get; set; }

    public DbContextMensajeria(DbContextOptions<DbContextMensajeria> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConfigServidor());
        modelBuilder.ApplyConfiguration(new ConfigMensaje());
        modelBuilder.ApplyConfiguration(new ConfigEstadisticasMensajes());
        modelBuilder.ApplyConfiguration(new ConfigCentroCostos());
        modelBuilder.ApplyConfiguration(new ConfigUnidadCostos());
        modelBuilder.ApplyConfiguration(new ConfigUsuarioCentroCostos());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        base.OnConfiguring(optionsBuilder);
    }
}
