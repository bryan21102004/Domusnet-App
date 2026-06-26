namespace DomusNet.API.DTOs;
public class ClienteReporteDto
{
public int IdCliente { get; set; }
public string NombreCompleto { get; set; } = string.Empty;
public string Direccion { get; set; } = string.Empty;
}