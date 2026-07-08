using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Models;

namespace DomusNet.API.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly DomusNetDBContext _context;

    public TicketsRepository(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estado = null, int? idAsignadoA = null)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync(
            "listarTickets",
            new
            {
                Estado = estado,
                IdAsignadoA = idAsignadoA
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dynamic?> BuscarAsync(int idTicket)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync(
            "buscarTicket",
            new
            {
                IdTicket = idTicket
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> ListarHistorialAsync(int idTicket)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync(
            "listarHistorialTicket",
            new
            {
                IdTicket = idTicket
            },
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

    public async Task<IEnumerable<ClienteCorreoDto>> ListarClientesActivosConCorreoAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<ClienteCorreoDto>(
            "listarClientesActivosConCorreo",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ClienteReporteDto?> BuscarClienteActivoPorTelefonoAsync(string telefono)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ClienteReporteDto>(
            "buscarClienteActivoPorTelefono",
            new
            {
                Telefono = telefono
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearReporteClienteAsync(
        string titulo,
        string descripcion,
        int? idCliente,
        int idUsuarioSistema)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<SpResultDto>(
            "nuevoTicket",
            new
            {
                Titulo = titulo,
                Descripcion = descripcion,
                Tipo = "Averia",
                Prioridad = "Alta",
                IdCliente = idCliente,
                IdCreadoPor = idUsuarioSistema,
                IdAsignadoA = (int?)null,
                EsGlobal = false
            },
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

    public async Task<ClienteVerificadoResponse?> VerificarClientePorContratoAsync(string numeroContrato)
{
    using var connection = _context.CreateConnection();

    return await connection.QueryFirstOrDefaultAsync<ClienteVerificadoResponse>(
        "dbo.verificarClientePorContrato",
        new { NumeroContrato = numeroContrato },
        commandType: CommandType.StoredProcedure
    );
}
}