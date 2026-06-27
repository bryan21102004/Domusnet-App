namespace DomusNet.Blazor.Models;

public class SolicitudServicioDto
{
    public int IdSolicitud { get; set; }
    public int IdClienteExterno { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }
    public int? IdPaquete { get; set; }
    public string? NombrePaquete { get; set; }
    public string? Estado { get; set; }
    public string? Notas { get; set; }
    public DateTime FechaSolicitud { get; set; }
}