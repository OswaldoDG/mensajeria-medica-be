using System.Diagnostics.CodeAnalysis;
using mensajeriamedica.model.comunicaciones.mensajes;
using mensajeriamedica.model.comunicaciones.servidores;
using Microsoft.EntityFrameworkCore;

namespace mensajeriamedica.services.comunicaciones;

/// <summary>
/// Contexto de mensajeria.
/// </summary>
[ExcludeFromCodeCoverage]
public class DbContextMensajeria: DbContext
{
    public DbSet<Servidor> Servidores { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }
    public DbSet<EstadisticasMensajes> EstadisticasMensajes { get; set; }
}
