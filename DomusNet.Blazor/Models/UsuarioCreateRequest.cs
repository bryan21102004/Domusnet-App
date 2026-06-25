namespace DomusNet.Blazor.Models;

public class UsuarioCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
}
