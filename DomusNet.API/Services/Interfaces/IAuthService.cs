using System.Threading.Tasks;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services.Interfaces;

public interface IAuthService
{
    Task<UsuarioLoginDto?> ValidateUserAsync(string correo, string password);
    string GenerateJwtToken(UsuarioLoginDto usuario);
    Task<string> GenerateRefreshTokenAsync(int idUsuario);
    Task ActualizarTokenAsync(int idUsuario, string refreshToken);
    Task<int?> VerificarRefreshTokenAsync(int idUsuario, string refreshToken);
    Task<(string newToken, string newRefresh)?> RefreshTokenAsync(int idUsuario, string refreshToken, UsuarioLoginDto usuario);
    Task LogoutAsync(int idUsuario);
    Task<UsuarioLoginDto?> GetUsuarioAsync(int idUsuario);
    string HashPassword(string password);
}
