
using System.Data;
using DomusNet.API.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DomusNet.API.Models;

namespace DomusNet.API.Data;

public class DomusNetDBContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DomusNetDBContext(DbContextOptions<DomusNetDBContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("ConexionSQL")!;
    }

    public DbSet<Rol>              Roles               => Set<Rol>();
    public DbSet<Usuario>          Usuarios            => Set<Usuario>();
    public DbSet<PaqueteServicio>  PaquetesServicio    => Set<PaqueteServicio>();
    public DbSet<ClienteExterno>   ClientesExternos    => Set<ClienteExterno>();
    public DbSet<SolicitudServicio> SolicitudesServicio => Set<SolicitudServicio>();
    public DbSet<Cliente>          Clientes            => Set<Cliente>();
    public DbSet<AsignacionPaquete> AsignacionesPaquete => Set<AsignacionPaquete>();
    public DbSet<Ticket>           Tickets             => Set<Ticket>();
    public DbSet<HistorialTicket>  HistorialTickets    => Set<HistorialTicket>();
    public DbSet<Notificacion>     Notificaciones      => Set<Notificacion>();
    public DbSet<Ingreso>          Ingresos            => Set<Ingreso>();
    public DbSet<Venta>            Ventas              => Set<Venta>();
    public DbSet<Reporte>          Reportes            => Set<Reporte>();
    public DbSet<AuditoriaAccion>  AuditoriaAcciones   => Set<AuditoriaAccion>();
public DbSet<InstalacionProgramada> InstalacionesProgramadas => Set<InstalacionProgramada>(); 

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Ticket>()
            .HasOne(t => t.CreadoPor)
            .WithMany()
            .HasForeignKey(t => t.IdCreadoPor)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Ticket>()
            .HasOne(t => t.AsignadoA)
            .WithMany()
            .HasForeignKey(t => t.IdAsignadoA)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<AsignacionPaquete>()
            .HasOne(a => a.Vendedor)
            .WithMany()
            .HasForeignKey(a => a.IdVendedor)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Cliente>()
            .HasOne(c => c.Vendedor)
            .WithMany()
            .HasForeignKey(c => c.IdVendedor)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<SolicitudServicio>()
            .HasOne(s => s.VendedorAsignado)
            .WithMany()
            .HasForeignKey(s => s.IdVendedorAsignado)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<HistorialTicket>()
            .HasOne(h => h.Usuario)
            .WithMany()
            .HasForeignKey(h => h.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<AuditoriaAccion>()
            .HasIndex(a => new { a.Tabla, a.Fecha });

        mb.Entity<Notificacion>()
            .HasIndex(n => new { n.IdUsuarioDestino, n.Leida });

        mb.Entity<Ticket>()
            .HasIndex(t => t.Estado);

        mb.Entity<Cliente>()
            .HasIndex(c => c.EstadoPago);

        mb.Entity<Rol>().HasData(
            new Rol { IdRol = 1, NombreRol = "Administrador", Descripcion = "Control total del sistema" },
            new Rol { IdRol = 2, NombreRol = "Vendedor",      Descripcion = "Gestión de clientes y paquetes" },
            new Rol { IdRol = 3, NombreRol = "Tecnico",       Descripcion = "Atención de tickets e incidencias" }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(_connectionString);
    }
}
