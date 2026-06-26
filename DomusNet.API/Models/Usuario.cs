using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Contrasena { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Required]
    [ForeignKey(nameof(Rol))]
    public int IdRol { get; set; }

    [Required]
    public bool Activo { get; set; } = true;

    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaModificacion { get; set; }

    [MaxLength(255)]
    public string? RefreshToken { get; set; }

    public Rol Rol { get; set; } = null!;
}
