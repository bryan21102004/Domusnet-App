using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class AuditoriaAccion
{
    [Key]
    public int IdAuditoriaAccion { get; set; }

    [Required]
    [MaxLength(100)]
    public string Tabla { get; set; } = string.Empty;

    public int? IdRegistro { get; set; }

    [Required]
    [MaxLength(50)]
    public string Accion { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(Usuario))]
    public int IdUsuario { get; set; }

    [Required]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [MaxLength(1000)]
    public string? Detalle { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
