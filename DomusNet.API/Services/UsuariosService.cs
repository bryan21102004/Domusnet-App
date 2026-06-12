using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services;

public class UsuariosService
{
    private readonly DomusNetDBContext _context;
    private readonly AuthService _authService;

    public UsuariosService(DomusNetDBContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ListarAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<UsuarioResponseDto>(
            "listarUsuarios", commandType: CommandType.StoredProcedure);
    }

    public async Task<UsuarioResponseDto?> BuscarAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UsuarioResponseDto>(
            "buscarUsuario",
            new { IdUsuario = idUsuario },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearAsync(UsuarioCreateDto dto)
    {
        using var connection = _context.CreateConnection();
        var hash = _authService.HashPassword(dto.Password);
        return await connection.QueryFirstAsync<SpResultDto>(
            "nuevoUsuario",
            new
            {
                dto.Nombre,
                dto.Correo,
                Contrasena = hash,
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

    public async Task<int> CambiarContrasenaAsync(int idUsuario, string password)
    {
        using var connection = _context.CreateConnection();
        var hash = _authService.HashPassword(password);
        return await connection.QueryFirstAsync<int>(
            "cambiarContrasenaUsuario",
            new { IdUsuario = idUsuario, Contrasena = hash },
            commandType: CommandType.StoredProcedure);
    }
}
