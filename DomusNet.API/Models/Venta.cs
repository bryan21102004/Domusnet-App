using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Venta
{
    [Key]
    public int IdVenta { get; set; }

    [Required]
    [ForeignKey(nameof(Asignacion))]
    public int IdAsignacion { get; set; }

    [Required]
    public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

    [Required]
    [MaxLength(20)]
    public string Estado { get; set; } = "Activa";

    [Column(TypeName = "decimal(5,2)")]
    public decimal? PorcentajeDistribucion { get; set; }

    public AsignacionPaquete Asignacion { get; set; } = null!;
}
