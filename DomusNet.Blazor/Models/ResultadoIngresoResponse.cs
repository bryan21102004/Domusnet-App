namespace DomusNet.Blazor.Models;

public class ResultadoIngresoResponse
{
    public int Resultado { get; set; }
    public int IdGenerado { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime? FechaProximoPago { get; set; }
}