namespace DomusNet.API.Models;

public class ClienteVerificadoResponse
{
    public int IdCliente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NumeroContrato { get; set; } = string.Empty;
}