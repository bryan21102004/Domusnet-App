using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class PaqueteServicio
{
    [Key]
    public int IdPaquete { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [MaxLength(50)]
    public string? Velocidad { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? PorcentajeDistribucion { get; set; }

    [Required]
    [MaxLength(20)]
    public string Estado { get; set; } = EstadoPaquete.Activo.ToString();

    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<AsignacionPaquete> Asignaciones { get; set; } = [];
    public ICollection<SolicitudServicio> Solicitudes { get; set; } = [];
}
