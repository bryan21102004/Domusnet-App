using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;

namespace DomusNet.API.Repositories;

public class SolicitudesRepository : ISolicitudesRepository
{
    private readonly DomusNetDBContext _context;

    public SolicitudesRepository(DomusNetDBContext context)
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

    public async Task<SpResultDto?> ConvertirSolicitudEnClienteAsync(
        int idSolicitud,
        int idVendedor,
        string? notas)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<SpResultDto>(
            "dbo.convertirSolicitudEnCliente",
            new
            {
                IdSolicitud = idSolicitud,
                IdVendedor = idVendedor,
                Notas = notas
            },
            commandType: CommandType.StoredProcedure);
    }
}