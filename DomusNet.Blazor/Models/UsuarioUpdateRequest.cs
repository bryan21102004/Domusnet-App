namespace DomusNet.Blazor.Models;

public class UsuarioUpdateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
    public bool Activo { get; set; } = true;
}
