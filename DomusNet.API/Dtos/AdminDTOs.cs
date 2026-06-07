namespace DomusNet.API.DTOs;

public class IngresoCreateDto
{
    public int? IdCliente { get; set; }
    public int? IdPaquete { get; set; }
    public decimal Monto { get; set; }
    public string? Descripcion { get; set; }
    public int IdRegistradoPor { get; set; }
}

public class ResumenIngresosDto
{
    public decimal TotalIngresos { get; set; }
    public int CantidadRegistros { get; set; }
}

public class DashboardDto
{
    public int TotalClientes { get; set; }
    public int TotalTickets { get; set; }
    public int TicketsAbiertos { get; set; }
    public int TicketsResueltos { get; set; }
    public decimal TotalIngresos { get; set; }
    public decimal IngresosMesActual { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int ClientesMorosos { get; set; }
}

public class ReporteClientesResumenDto
{
    public int TotalClientes { get; set; }
    public int AlDia { get; set; }
    public int Morosos { get; set; }
    public int Pendientes { get; set; }
}

public class ReporteTicketsResumenDto
{
    public int Total { get; set; }
    public int Pendientes { get; set; }
    public int EnProceso { get; set; }
    public int Resueltos { get; set; }
}

public class ReporteIngresosResumenDto
{
    public decimal TotalPeriodo { get; set; }
    public int Cantidad { get; set; }
}

public class GuardarReporteDto
{
    public string Tipo { get; set; } = string.Empty;
    public string? Parametros { get; set; }
}
