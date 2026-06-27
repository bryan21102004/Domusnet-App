using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Reporte
{
    [Key]
    public int IdReporte { get; set; }

    [Required]
    [MaxLength(30)]
    public string Tipo { get; set; } = TipoReporte.Operativo.ToString();

    [MaxLength(1000)]
    public string? Parametros { get; set; }

    [Required]
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

    [Required]
    [ForeignKey(nameof(GeneradoPor))]
    public int IdGeneradoPor { get; set; }

    public Usuario GeneradoPor { get; set; } = null!;
}
