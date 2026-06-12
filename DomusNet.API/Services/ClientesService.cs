using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services;

public class ClientesService
{
    private readonly DomusNetDBContext _context;

    public ClientesService(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estadoPago = null)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(
            "listarClientes",
            new { EstadoPago = estadoPago },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dynamic?> BuscarAsync(int idCliente)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync(
            "buscarCliente",
            new { IdCliente = idCliente },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearAsync(ClienteCreateDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<SpResultDto>(
            "nuevoCliente",
            dto,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EditarAsync(int idCliente, ClienteUpdateDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<int>(
            "editarCliente",
            new
            {
                IdCliente = idCliente,
                dto.NombreCompleto,
                dto.Telefono,
                dto.Correo,
                dto.Direccion,
                dto.EstadoPago
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> AsignarPaqueteAsync(AsignarPaqueteDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<SpResultDto>(
            "asignarPaqueteCliente",
            dto,
            commandType: CommandType.StoredProcedure);
    }
}
