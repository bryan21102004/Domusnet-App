namespace DomusNet.API.Dtos;
public class CompletarInstalacionDto
{
    public string FotoEvidencia { get; set; } = string.Empty;
    public string PruebaVelocidad { get; set; } = string.Empty;
    public string? ComentarioTecnico { get; set; }
}