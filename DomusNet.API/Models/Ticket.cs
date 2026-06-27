using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Ticket
{
    [Key]
    public int IdTicket { get; set; }

    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    [Required]
    [MaxLength(20)]
    public string Tipo { get; set; } = TipoTicket.Averia.ToString();

    [Required]
    [MaxLength(20)]
    public string Estado { get; set; } = EstadoTicket.Pendiente.ToString();

    [Required]
    [MaxLength(10)]
    public string Prioridad { get; set; } = PrioridadTicket.Media.ToString();

    [ForeignKey(nameof(Cliente))]
    public int? IdCliente { get; set; }

    [Required]
    [ForeignKey(nameof(CreadoPor))]
    public int IdCreadoPor { get; set; }

    [ForeignKey(nameof(AsignadoA))]
    public int? IdAsignadoA { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }

    public DateTime? FechaCierre { get; set; }

    [Required]
    public bool EsGlobal { get; set; }

    public Cliente? Cliente { get; set; }
    public Usuario CreadoPor { get; set; } = null!;
    public Usuario? AsignadoA { get; set; }
    public ICollection<HistorialTicket> Historial { get; set; } = [];
    public ICollection<Notificacion> Notificaciones { get; set; } = [];
}
