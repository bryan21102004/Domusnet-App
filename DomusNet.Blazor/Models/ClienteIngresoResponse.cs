public class ClienteIngresoResponse
{
    public int IdCliente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string EstadoPago { get; set; } = string.Empty;

    public int IdAsignacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string EstadoAsignacion { get; set; } = string.Empty;

    public int IdPaquete { get; set; }
    public string NombrePaquete { get; set; } = string.Empty;
    public string? Velocidad { get; set; }
    public decimal Precio { get; set; }
}