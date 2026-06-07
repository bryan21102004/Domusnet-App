using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class HistorialTicket
{
    [Key]
    public int IdHistorial { get; set; }

    [Required]
    [ForeignKey(nameof(Ticket))]
    public int IdTicket { get; set; }

    [MaxLength(20)]
    public string? EstadoAnterior { get; set; }

    [Required]
    [MaxLength(20)]
    public string EstadoNuevo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Comentario { get; set; }

    [Required]
    [ForeignKey(nameof(Usuario))]
    public int IdUsuario { get; set; }

    [Required]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
