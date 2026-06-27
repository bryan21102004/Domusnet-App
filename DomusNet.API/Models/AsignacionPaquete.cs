using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class AsignacionPaquete
{
    [Key]
    public int IdAsignacion { get; set; }

    [Required]
    [ForeignKey(nameof(Cliente))]
    public int IdCliente { get; set; }

    [Required]
    [ForeignKey(nameof(Paquete))]
    public int IdPaquete { get; set; }

    [Required]
    [ForeignKey(nameof(Vendedor))]
    public int IdVendedor { get; set; }

    [Required]
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaVencimiento { get; set; }

    [Required]
    [MaxLength(20)]
    public string Estado { get; set; } = EstadoAsignacion.Activa.ToString();

    public Cliente Cliente { get; set; } = null!;
    public PaqueteServicio Paquete { get; set; } = null!;
    public Usuario Vendedor { get; set; } = null!;
    public ICollection<Venta> Ventas { get; set; } = [];
}
