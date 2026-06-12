using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services;

public class ReportesService
{
    private readonly DomusNetDBContext _context;

    public ReportesService(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> ObtenerDashboardAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<DashboardDto>(
            "obtenerDashboard",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<object> ReporteIngresosAsync(DateTime? desde, DateTime? hasta)
    {
        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(
            "reporteIngresos",
            new { Desde = desde, Hasta = hasta },
            commandType: CommandType.StoredProcedure);

        var detalle = (await multi.ReadAsync<dynamic>()).ToList();
        var resumen = await multi.ReadFirstAsync<ReporteIngresosResumenDto>();
        return new { detalle, resumen };
    }

    public async Task<object> ReporteClientesAsync(string? estadoPago)
    {
        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(
            "reporteClientes",
            new { EstadoPago = estadoPago },
            commandType: CommandType.StoredProcedure);

        var detalle = (await multi.ReadAsync<dynamic>()).ToList();
        var resumen = await multi.ReadFirstAsync<ReporteClientesResumenDto>();
        return new { detalle, resumen };
    }

    public async Task<object> ReporteTicketsAsync(
        string? estado, string? tipo, DateTime? desde, DateTime? hasta)
    {
        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(
            "reporteTickets",
            new { Estado = estado, Tipo = tipo, Desde = desde, Hasta = hasta },
            commandType: CommandType.StoredProcedure);

        var detalle = (await multi.ReadAsync<dynamic>()).ToList();
        var resumen = await multi.ReadFirstAsync<ReporteTicketsResumenDto>();
        return new { detalle, resumen };
    }

    public async Task<SpResultDto> GuardarReporteAsync(GuardarReporteDto dto, int idGeneradoPor)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<SpResultDto>(
            "guardarReporte",
            new { dto.Tipo, dto.Parametros, IdGeneradoPor = idGeneradoPor },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> ListarGeneradosAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(
            "listarReportesGenerados",
            commandType: CommandType.StoredProcedure);
    }
}
