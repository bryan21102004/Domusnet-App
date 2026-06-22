using System.Data;
using System.Text.Json;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Models;

namespace DomusNet.API.Services;

public class ReportesService : Interfaces.IReportesService
{
    private readonly DomusNetDBContext _context;

    public ReportesService(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<ResultadoOperacion> GuardarConfiguracionDistribucionAsync(
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

        var resultado = await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "guardarConfiguracionDistribucion",
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

        return resultado ?? new ResultadoOperacion
        {
            Resultado = -1,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }

    public async Task<ResultadoOperacion> GenerarReporteIngresosAsync(
        GenerarReporteIngresosRequest request)
    {
        using var connection = _context.CreateConnection();

        var resultado = await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "generarReporteIngresosMensual",
            new
            {
                request.Mes,
                request.Anio,
                request.Quincena,
                request.IdRegistradoPor,
                request.Notas
            },
            commandType: CommandType.StoredProcedure);

        return resultado ?? new ResultadoOperacion
        {
            Resultado = -1,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }
    public async Task<DetalleReporteIngresoResponse?> ObtenerDetalleReporteIngresoAsync(int idIngresoMensual)
{
    using var connection = _context.CreateConnection();

    using var multi = await connection.QueryMultipleAsync(
        "dbo.obtenerDetalleReporteIngreso",
        new { IdIngresoMensual = idIngresoMensual },
        commandType: CommandType.StoredProcedure);

    var resumen = await multi.ReadFirstOrDefaultAsync<ReporteIngresoResumen>();
    var distribuciones = await multi.ReadAsync<DistribucionIngresoDetalle>();

    if (resumen == null)
    {
        return null;
    }

    return new DetalleReporteIngresoResponse
    {
        Resumen = resumen,
        Distribuciones = distribuciones
    };
}
}