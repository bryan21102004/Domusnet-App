namespace DomusNet.Blazor.Models;

public class SolicitudPaqueteRequest
{
    public int IdPaquete { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string? Notas { get; set; }
}