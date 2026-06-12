namespace DomusNet.API.Models;

public class ResultadoOperacion
{
    public int Resultado { get; set; }
    public int? IdInstalacion { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}