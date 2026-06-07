using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services;

public class SolicitudesService
{
    private readonly DomusNetDbContext _context;

    public SolicitudesService(DomusNetDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitudResultDto> CrearAsync(SolicitudCreateDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<SolicitudResultDto>(
            "nuevaSolicitud",
            dto,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estado = null)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(
            "listarSolicitudes",
            new { Estado = estado },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> AtenderAsync(int idSolicitud, AtenderSolicitudDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<int>(
            "atenderSolicitud",
            new
            {
                IdSolicitud = idSolicitud,
                dto.IdVendedorAsignado,
                dto.Notas
            },
            commandType: CommandType.StoredProcedure);
    }
}
