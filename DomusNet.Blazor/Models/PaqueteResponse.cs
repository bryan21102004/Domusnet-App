namespace DomusNet.Blazor.Models;

public class PaqueteResponse
{
    public int IdPaquete { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Velocidad { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public decimal? PorcentajeDistribucion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}