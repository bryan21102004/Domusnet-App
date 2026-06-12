using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Models;

[Table("InstalacionesProgramadas")]
public class InstalacionProgramada
{
    [Key]
    public int IdInstalacion { get; set; }

    public int IdSolicitud { get; set; }

    public int IdTecnicoAsignado { get; set; }

    public int IdVendedorPrograma { get; set; }

    public DateTime FechaProgramada { get; set; }

    public DateTime? FechaRealizacion { get; set; }

    public string? UbicacionInstalacion { get; set; }

    public string? FotoEvidencia { get; set; }

    public string? PruebaVelocidad { get; set; }

    public string Estado { get; set; } = "Programada";

    public string? NotasVendedor { get; set; }

    public string? ComentarioTecnico { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaActualizacion { get; set; }
}