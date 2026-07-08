namespace DomusNet.Blazor.Models;

public class ClienteResponse
{
    public int IdCliente { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string Direccion { get; set; } = string.Empty;

    public string EstadoPago { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    public string? NombreVendedor { get; set; }

    public string? NombrePaquete { get; set; }
    public string? Velocidad { get; set; }
    public decimal? Precio { get; set; }

    public DateTime? FechaAsignacion { get; set; }
    public string? EstadoAsignacion { get; set; }
    public string? NumeroContrato { get; set; }

}