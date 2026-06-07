using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Notificacion
{
    [Key]
    public int IdNotificacion { get; set; }

    [Required]
    [ForeignKey(nameof(UsuarioDestino))]
    public int IdUsuarioDestino { get; set; }

    [ForeignKey(nameof(Ticket))]
    public int? IdTicket { get; set; }

    [Required]
    [MaxLength(500)]
    public string Mensaje { get; set; } = string.Empty;

    [Required]
    public bool Leida { get; set; }

    [Required]
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    public Usuario UsuarioDestino { get; set; } = null!;
    public Ticket? Ticket { get; set; }
}
