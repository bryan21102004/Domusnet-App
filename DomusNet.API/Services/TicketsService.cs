using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services;

public class TicketsService
{
    private readonly DomusNetDbContext _context;

    public TicketsService(DomusNetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estado = null, int? idAsignadoA = null)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(
            "listarTickets",
            new { Estado = estado, IdAsignadoA = idAsignadoA },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dynamic?> BuscarAsync(int idTicket)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync(
            "buscarTicket",
            new { IdTicket = idTicket },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> ListarHistorialAsync(int idTicket)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(
            "listarHistorialTicket",
            new { IdTicket = idTicket },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> ListarGlobalesAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(
            "listarTicketsGlobales",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearAsync(TicketCreateDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<SpResultDto>(
            "nuevoTicket",
            dto,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarEstadoAsync(int idTicket, ActualizarEstadoTicketDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<int>(
            "actualizarEstadoTicket",
            new
            {
                IdTicket = idTicket,
                dto.EstadoNuevo,
                dto.IdUsuario,
                dto.Comentario
            },
            commandType: CommandType.StoredProcedure);
    }
}
