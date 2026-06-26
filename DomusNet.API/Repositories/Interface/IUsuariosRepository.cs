using DomusNet.API.DTOs;

namespace DomusNet.API.Repositories.Interfaces;

public interface IUsuariosRepository
{
    Task<IEnumerable<UsuarioResponseDto>> ListarAsync();

    Task<UsuarioResponseDto?> BuscarAsync(int idUsuario);

    Task<SpResultDto> CrearAsync(UsuarioCreateDto dto, string contrasenaHash);

    Task<int> EditarAsync(int idUsuario, UsuarioUpdateDto dto);

    Task<int> EliminarAsync(int idUsuario);

    Task<int> CambiarContrasenaAsync(int idUsuario, string contrasenaHash);

    Task<ValidacionDesactivacionDto> ValidarDesactivacionAsync(int idUsuario);
}