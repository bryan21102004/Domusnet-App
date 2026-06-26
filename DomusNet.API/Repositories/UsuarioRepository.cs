using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;

namespace DomusNet.API.Repositories;

public class UsuariosRepository : IUsuariosRepository
{
    private readonly DomusNetDBContext _context;

    public UsuariosRepository(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ListarAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<UsuarioResponseDto>(
            "listarUsuarios",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<UsuarioResponseDto?> BuscarAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<UsuarioResponseDto>(
            "buscarUsuario",
            new { IdUsuario = idUsuario },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearAsync(UsuarioCreateDto dto, string contrasenaHash)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<SpResultDto>(
            "nuevoUsuario",
            new
            {
                dto.Nombre,
                dto.Correo,
                Contrasena = contrasenaHash,
                dto.Telefono,
                dto.IdRol
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EditarAsync(int idUsuario, UsuarioUpdateDto dto)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<int>(
            "editarUsuario",
            new
            {
                IdUsuario = idUsuario,
                dto.Nombre,
                dto.Correo,
                dto.Telefono,
                dto.IdRol,
                dto.Activo
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<int>(
            "eliminarUsuario",
            new { IdUsuario = idUsuario },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CambiarContrasenaAsync(int idUsuario, string contrasenaHash)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<int>(
            "cambiarContrasenaUsuario",
            new
            {
                IdUsuario = idUsuario,
                Contrasena = contrasenaHash
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ValidacionDesactivacionDto> ValidarDesactivacionAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ValidacionDesactivacionDto>(
            "validarDesactivacionUsuario",
            new { IdUsuario = idUsuario },
            commandType: CommandType.StoredProcedure)
            ?? new ValidacionDesactivacionDto();
    }
}