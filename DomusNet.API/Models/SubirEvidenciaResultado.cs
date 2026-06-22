namespace DomusNet.API.Models;

public class SubirEvidenciaResultado
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string? Ruta { get; set; }
}