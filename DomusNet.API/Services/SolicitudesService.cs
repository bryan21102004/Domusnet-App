using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace DomusNet.API.Services;

public class SolicitudesService
{
    private readonly DomusNetDBContext _context;

    public SolicitudesService(DomusNetDBContext context)
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
 public async Task<SpResultDto> ConvertirSolicitudEnClienteAsync(
        int idSolicitud,
        int idVendedor,
        string? notas)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var resultado = await connection.QueryFirstOrDefaultAsync<SpResultDto>(
            "convertirSolicitudEnCliente",
            new
            {
                IdSolicitud = idSolicitud,
                IdVendedor = idVendedor,
                Notas = notas
            },
            commandType: CommandType.StoredProcedure
        );

        return resultado ?? new SpResultDto
        {
            Resultado = 0,
            IdGenerado = 0
        };
    }
}
