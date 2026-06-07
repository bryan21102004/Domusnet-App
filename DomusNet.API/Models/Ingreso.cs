using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Ingreso
{
    [Key]
    public int IdIngreso { get; set; }

    [ForeignKey(nameof(Cliente))]
    public int? IdCliente { get; set; }

    [ForeignKey(nameof(Paquete))]
    public int? IdPaquete { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

    [Required]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    [Required]
    [ForeignKey(nameof(RegistradoPor))]
    public int IdRegistradoPor { get; set; }

    public Cliente? Cliente { get; set; }
    public PaqueteServicio? Paquete { get; set; }
    public Usuario RegistradoPor { get; set; } = null!;
}
