namespace DomusNet.API.Models;

public class GenerarReporteIngresosRequest
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public string? Quincena { get; set; }
    public int IdRegistradoPor { get; set; }
    public string? Notas { get; set; }
}