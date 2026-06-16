namespace DomusNet.Blazor.Models;

public class InstalacionTecnicoResponse
{
    public int IdInstalacion { get; set; }
    public int IdSolicitud { get; set; }

    public int IdTecnicoAsignado { get; set; }
    public int IdVendedorPrograma { get; set; }

    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaRealizacion { get; set; }

    public string? UbicacionInstalacion { get; set; }
    public string? FotoEvidencia { get; set; }
    public string? PruebaVelocidad { get; set; }

    public string? Estado { get; set; }
    public string? NotasVendedor { get; set; }
    public string? ComentarioTecnico { get; set; }

    public string? NombreCliente { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }

    public string? NombrePaquete { get; set; }
}