using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Repositories.Interfaces;

namespace DomusNet.API.Repositories;

public class NotificacionesRepository : INotificacionesRepository
{
    private readonly DomusNetDBContext _context;

    public NotificacionesRepository(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(int idUsuario, bool soloNoLeidas = false)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync(
            "listarNotificaciones",
            new
            {
                IdUsuarioDestino = idUsuario,
                SoloNoLeidas = soloNoLeidas
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ContarNoLeidasAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<int>(
            "contarNotificacionesNoLeidas",
            new
            {
                IdUsuarioDestino = idUsuario
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> MarcarLeidaAsync(int idNotificacion, int idUsuario)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<int>(
            "marcarNotificacionLeida",
            new
            {
                IdNotificacion = idNotificacion,
                IdUsuarioDestino = idUsuario
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> MarcarTodasLeidasAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<int>(
            "marcarTodasNotificacionesLeidas",
            new
            {
                IdUsuarioDestino = idUsuario
            },
            commandType: CommandType.StoredProcedure);
    }
}