namespace DomusNet.Blazor.Models;

public class GuardarConfiguracionDistribucionRequest
{
    public string Nombre { get; set; } = string.Empty;
    public decimal PorcentajeDomusNet { get; set; }
    public decimal PorcentajeIVA { get; set; }
    public decimal PorcentajeCruzRoja { get; set; }
    public decimal Porcentaje911 { get; set; }
    public int IdCreadoPor { get; set; }

    public List<TrabajadorDistribucionRequest> Trabajadores { get; set; } = new();
}

public class TrabajadorDistribucionRequest
{
    public int IdUsuario { get; set; }
    public string TipoDistribucion { get; set; } = "Otro";
    public decimal Porcentaje { get; set; }
}

public class GenerarReporteIngresosRequest
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public string? Quincena { get; set; }
    public int IdRegistradoPor { get; set; }
    public string? Notas { get; set; }
}

public class ResultadoOperacion
{
    public int Resultado { get; set; }
    public string? Mensaje { get; set; }
    public int IdGenerado { get; set; }
}

public class DetalleReporteIngresoResponse
{
    public ReporteIngresoResumen? Resumen { get; set; }
    public List<DistribucionIngresoDetalle> Distribuciones { get; set; } = new();
    public List<DistribucionPorTipoResumen> TotalesPorTipo { get; set; } = new();
}

public class ReporteIngresoResumen
{
    public int IdIngresoMensual { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    public string? Quincena { get; set; }

    public decimal MontoTotalBruto { get; set; }
    public decimal MontoTotalConRebajas { get; set; }

    public decimal PorcentajeDomusNet { get; set; }
    public decimal MontoDomusNet { get; set; }

    public decimal PorcentajeTrabajadores { get; set; }
    public decimal MontoTrabajadores { get; set; }

    public decimal PorcentajeIVA { get; set; }
    public decimal PorcentajeCruzRoja { get; set; }
    public decimal Porcentaje911 { get; set; }

    public string? Notas { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? RegistradoPor { get; set; }
}

public class DistribucionIngresoDetalle
{
    public int IdDistribucion { get; set; }
    public int IdIngresoMensual { get; set; }
    public int IdUsuario { get; set; }

    public string? Trabajador { get; set; }
    public string? TipoDistribucion { get; set; }

    public decimal PorcentajeAplicado { get; set; }
    public decimal MontoAsignado { get; set; }
}

public class DistribucionPorTipoResumen
{
    public string? TipoDistribucion { get; set; }
    public decimal TotalAsignado { get; set; }
}