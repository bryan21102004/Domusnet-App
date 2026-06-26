namespace DomusNet.Blazor.Models;

public class CompletarInstalacionRequest
{
    public int IdInstalacion { get; set; }
    public string FotoEvidencia { get; set; } = string.Empty;
    public string PruebaVelocidad { get; set; } = string.Empty;
    public string? ComentarioTecnico { get; set; }
}