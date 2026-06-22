using System.Collections.Generic;
using System.Threading.Tasks;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services.Interfaces;

public interface IUsuariosService
{
    Task<IEnumerable<UsuarioResponseDto>> ListarAsync();
    Task<UsuarioResponseDto?> BuscarAsync(int idUsuario);
    Task<SpResultDto> CrearAsync(UsuarioCreateDto dto);
    Task<int> EditarAsync(int idUsuario, UsuarioUpdateDto dto);
    Task<int> EliminarAsync(int idUsuario);
    Task<int> CambiarContrasenaAsync(int idUsuario, string password);
}
