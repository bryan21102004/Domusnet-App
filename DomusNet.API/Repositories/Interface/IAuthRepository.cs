using DomusNet.API.DTOs;

namespace DomusNet.API.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<UsuarioLoginDto?> BuscarUsuarioLoginAsync(string correo);

    Task ActualizarTokenAsync(int idUsuario, string refreshToken);

    Task<int?> VerificarRefreshTokenAsync(int idUsuario, string refreshToken);

    Task<UsuarioResponseDto?> BuscarUsuarioAsync(int idUsuario);
}