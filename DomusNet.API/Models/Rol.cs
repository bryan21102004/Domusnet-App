using System.ComponentModel.DataAnnotations;

namespace DomusNet.API.Data.Models;

public class Rol
{
    [Key]
    public int IdRol { get; set; }

    [Required]
    [MaxLength(50)]
    public string NombreRol { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Descripcion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = [];
}
