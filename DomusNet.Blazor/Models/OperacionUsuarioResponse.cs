namespace DomusNet.Blazor.Models;

public class OperacionUsuarioResponse
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int IdGenerado { get; set; }
}
