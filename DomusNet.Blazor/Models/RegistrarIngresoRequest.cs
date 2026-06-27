namespace DomusNet.Blazor.Models;

public class RegistrarIngresoRequest
{
    public int IdCliente { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string? ReferenciaPago { get; set; }
    public string? Descripcion { get; set; }
    public int IdRegistradoPor { get; set; }
}