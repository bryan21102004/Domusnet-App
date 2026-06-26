namespace DomusNet.API.Models;

public class CompletarInstalacionRequest
{
    public int IdInstalacion { get; set; }
    public string? FotoEvidencia { get; set; }
    public string? PruebaVelocidad { get; set; }
    public string? ComentarioTecnico { get; set; }
}
