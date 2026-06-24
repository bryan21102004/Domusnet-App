using System.Data;
using System.Text.Json;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Models;
using DomusNet.API.Repositories.Interfaces;

namespace DomusNet.API.Repositories;

public class ReportesRepository : IReportesRepository
{
    private readonly DomusNetDBContext _context;

    public ReportesRepository(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<ResultadoOperacion?> GuardarConfiguracionDistribucionAsync(
        GuardarConfiguracionDistribucionRequest request)
    {
        using var connection = _context.CreateConnection();

        var trabajadoresJson = JsonSerializer.Serialize(
            request.Trabajadores,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );

        return await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "dbo.guardarConfiguracionDistribucion",
            new
            {
                request.Nombre,
                request.PorcentajeDomusNet,
                request.PorcentajeIVA,
                request.PorcentajeCruzRoja,
                request.Porcentaje911,
                request.IdCreadoPor,
                TrabajadoresJson = trabajadoresJson
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ResultadoOperacion?> GenerarReporteIngresosAsync(
        GenerarReporteIngresosRequest request)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "dbo.generarReporteIngresosMensual",
            new
            {
                request.Mes,
                request.Anio,
                request.Quincena,
                request.IdRegistradoPor,
                request.Notas
            },
            commandType: CommandType.StoredProcedure);
    }
public async Task<DetalleReporteIngresoResponse?> ObtenerDetalleReporteIngresoAsync(
    int idIngresoMensual)
{
    using var connection = _context.CreateConnection();

    using var multi = await connection.QueryMultipleAsync(
        "dbo.obtenerDetalleReporteIngreso",
        new
        {
            IdIngresoMensual = idIngresoMensual
        },
        commandType: CommandType.StoredProcedure);

    var resumen = await multi.ReadFirstOrDefaultAsync<ReporteIngresoResumen>();

    if (resumen == null)
    {
        return null;
    }

    var distribuciones = await multi.ReadAsync<DistribucionIngresoDetalle>();

    var totalesPorTipo = await multi.ReadAsync<DistribucionPorTipoResumen>();

    return new DetalleReporteIngresoResponse
    {
        Resumen = resumen,
        Distribuciones = distribuciones,
        TotalesPorTipo = totalesPorTipo
    };
}
  
}