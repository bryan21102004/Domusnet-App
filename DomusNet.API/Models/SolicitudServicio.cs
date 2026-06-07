using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class SolicitudServicio
{
    [Key]
    public int IdSolicitud { get; set; }

    [Required]
    [ForeignKey(nameof(ClienteExterno))]
    public int IdClienteExterno { get; set; }

    [ForeignKey(nameof(Paquete))]
    public int? IdPaquete { get; set; }

    [Required]
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(30)]
    public string Estado { get; set; } = EstadoSolicitud.Pendiente.ToString();

    [ForeignKey(nameof(VendedorAsignado))]
    public int? IdVendedorAsignado { get; set; }

    [MaxLength(500)]
    public string? Notas { get; set; }

    public ClienteExterno ClienteExterno { get; set; } = null!;
    public PaqueteServicio? Paquete { get; set; }
    public Usuario? VendedorAsignado { get; set; }
    public ICollection<Cliente> ClientesOrigen { get; set; } = [];
}
