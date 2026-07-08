namespace DomusNet.API.Models;

public class IngresoResponse
{
    public int IdIngreso { get; set; }

    public int? IdCliente { get; set; }
    public string? NombreCliente { get; set; }
      public string? EstadoPago { get; set; }

    public int? IdPaquete { get; set; }
    public string? NombrePaquete { get; set; }
    public string? Velocidad { get; set; }

    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string? Descripcion { get; set; }

    public string? TipoIngreso { get; set; }
    public string? MetodoPago { get; set; }
    public string? Estado { get; set; }
    public string? ReferenciaPago { get; set; }
    public DateTime? FechaProximoPago { get; set; }

    public int IdRegistradoPor { get; set; }
    public string? NombreRegistradoPor { get; set; }
    public string? NumeroContrato { get; set; }
}