namespace DomusNet.API.Models;

public class GuardarConfiguracionDistribucionRequest
{
    public string Nombre { get; set; } = string.Empty;
    public decimal PorcentajeDomusNet { get; set; }
    public decimal PorcentajeIVA { get; set; }
    public decimal PorcentajeCruzRoja { get; set; }
    public decimal Porcentaje911 { get; set; }
    public int IdCreadoPor { get; set; }
    public List<TrabajadorDistribucionRequest> Trabajadores { get; set; } = new();
}