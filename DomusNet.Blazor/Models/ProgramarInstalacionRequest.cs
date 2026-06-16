namespace DomusNet.Blazor.Models;

public class ProgramarInstalacionRequest
{



    public int IdSolicitud { get; set; }
    public int IdTecnicoAsignado { get; set; }
    public int IdVendedorPrograma { get; set; }
    public DateTime FechaProgramada { get; set; }
    public string? UbicacionInstalacion { get; set; }
    public string? NotasVendedor { get; set; }
}