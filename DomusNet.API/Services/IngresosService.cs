using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services;

public class IngresosService
{
    private readonly DomusNetDbContext _context;

    public IngresosService(DomusNetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(
            "listarIngresos",
            new { Desde = desde, Hasta = hasta },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dynamic?> BuscarAsync(int idIngreso)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync(
            "buscarIngreso",
            new { IdIngreso = idIngreso },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearAsync(IngresoCreateDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<SpResultDto>(
            "nuevoIngreso",
            dto,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ResumenIngresosDto> ResumenAsync(int? mes = null, int? anio = null)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<ResumenIngresosDto>(
            "resumenIngresos",
            new { Mes = mes, Anio = anio },
            commandType: CommandType.StoredProcedure);
    }
}
