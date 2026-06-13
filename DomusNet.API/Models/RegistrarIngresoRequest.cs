namespace DomusNet.API.Models;

public class RegistrarIngresoRequest
{
    public int? IdCliente { get; set; }
    public int? IdPaquete { get; set; }
    public decimal Monto { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Descripcion { get; set; }
    public int IdRegistradoPor { get; set; }
    public string TipoIngreso { get; set; } = string.Empty;
    public string? MetodoPago { get; set; }
    public string? Quincena { get; set; }
}