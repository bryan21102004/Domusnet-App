
namespace DomusNet.Blazor.Models;

public class TecnicoResponse
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
}