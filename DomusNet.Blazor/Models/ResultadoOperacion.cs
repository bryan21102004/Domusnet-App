namespace DomusNet.Blazor.Models;

public class ResultadoOperacion
{
    public int Resultado { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int? IdGenerado { get; set; }
}