using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    [Required]
    [MaxLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Correo { get; set; }

    [Required]
    [MaxLength(300)]
    public string Direccion { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string EstadoPago { get; set; } = DomusNet.API.Data.Models.EstadoPago.AlDia.ToString();

    [Required]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
public string? NumeroContrato { get; set; }


    [Required]
    [ForeignKey(nameof(Vendedor))]
    public int IdVendedor { get; set; }

    [ForeignKey(nameof(SolicitudOrigen))]
    public int? IdSolicitudOrigen { get; set; }

    [Required]
    public bool Activo { get; set; } = true;

    public Usuario Vendedor { get; set; } = null!;
    public SolicitudServicio? SolicitudOrigen { get; set; }
    public ICollection<AsignacionPaquete> Asignaciones { get; set; } = [];
    public ICollection<Ticket> Tickets { get; set; } = [];
    public ICollection<Ingreso> Ingresos { get; set; } = [];
}
