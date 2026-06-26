using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;

namespace DomusNet.API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly DomusNetDBContext _context;

    public AuthRepository(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<UsuarioLoginDto?> BuscarUsuarioLoginAsync(string correo)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<UsuarioLoginDto>(
            "buscarUsuarioLogin",
            new { Correo = correo },
            commandType: CommandType.StoredProcedure);
    }

    public async Task ActualizarTokenAsync(int idUsuario, string refreshToken)
    {
        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            "modificarToken",
            new
            {
                IdUsuario = idUsuario,
                RefreshToken = refreshToken
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int?> VerificarRefreshTokenAsync(int idUsuario, string refreshToken)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<int?>(
            "verificarTokenR",
            new
            {
                IdUsuario = idUsuario,
                RefreshToken = refreshToken
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<UsuarioResponseDto?> BuscarUsuarioAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<UsuarioResponseDto>(
            "buscarUsuario",
            new { IdUsuario = idUsuario },
            commandType: CommandType.StoredProcedure);
    }
}