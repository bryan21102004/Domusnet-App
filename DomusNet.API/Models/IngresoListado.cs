namespace DomusNet.API.Models;

public class IngresoListado
{
    public int IdIngreso { get; set; }
    public int? IdCliente { get; set; }
    public string? Cliente { get; set; }
    public int? IdPaquete { get; set; }
    public string? Paquete { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string? Descripcion { get; set; }
    public string? TipoIngreso { get; set; }
    public string? MetodoPago { get; set; }
    public string? Quincena { get; set; }
    public string? Estado { get; set; }
    public int IdRegistradoPor { get; set; }
    public string? RegistradoPor { get; set; }
}