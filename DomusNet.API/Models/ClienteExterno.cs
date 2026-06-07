using System.ComponentModel.DataAnnotations;

namespace DomusNet.API.Data.Models;

public class ClienteExterno
{
    [Key]
    public int IdClienteExterno { get; set; }

    [Required]
    [MaxLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Telefono { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Correo { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Direccion { get; set; } = string.Empty;

    public ICollection<SolicitudServicio> Solicitudes { get; set; } = [];
}
