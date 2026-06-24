namespace DomusNet.API.Models;

public class ReportePdfResultado
{
    public byte[] Archivo { get; set; } = Array.Empty<byte>();
    public string NombreArchivo { get; set; } = "reporte.pdf";
}