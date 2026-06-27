using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomusNet.API.Data.Models;

public class Ingreso
{
    [Key]
    public int IdIngreso { get; set; }

    public int? IdCliente { get; set; }
    public int? IdPaquete { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; }

    public string? Descripcion { get; set; }

    public int IdRegistradoPor { get; set; }

    public string? TipoIngreso { get; set; }
    public string? MetodoPago { get; set; }
    public string? Estado { get; set; }
    public string? ReferenciaPago { get; set; }
    public DateTime? FechaProximoPago { get; set; }
}