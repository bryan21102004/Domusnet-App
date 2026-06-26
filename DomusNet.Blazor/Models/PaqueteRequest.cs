namespace DomusNet.Blazor.Models;

public class PaqueteCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Velocidad { get; set; }
    public decimal Precio { get; set; }
    public decimal? PorcentajeDistribucion { get; set; }
}

public class PaqueteUpdateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Velocidad { get; set; }
    public decimal Precio { get; set; }
    public decimal? PorcentajeDistribucion { get; set; }
    public string Estado { get; set; } = "Activo";
}

public class OperacionPaqueteResponse
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int? IdGenerado { get; set; }
}
