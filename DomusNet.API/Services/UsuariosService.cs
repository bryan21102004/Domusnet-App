using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class UsuariosService : IUsuariosService
{
    private readonly IUsuariosRepository _repository;
    private readonly IAuthService _authService;

    public UsuariosService(
        IUsuariosRepository repository,
        IAuthService authService)
    {
        _repository = repository;
        _authService = authService;
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ListarAsync()
    {
        return await _repository.ListarAsync();
    }

    public async Task<UsuarioResponseDto?> BuscarAsync(int idUsuario)
    {
        return await _repository.BuscarAsync(idUsuario);
    }

    public async Task<SpResultDto> CrearAsync(UsuarioCreateDto dto)
    {
        var hash = _authService.HashPassword(dto.Password);

        return await _repository.CrearAsync(dto, hash);
    }

    public async Task<int> EditarAsync(int idUsuario, UsuarioUpdateDto dto)
    {
        return await _repository.EditarAsync(idUsuario, dto);
    }

    public async Task<int> EliminarAsync(int idUsuario)
    {
        return await _repository.EliminarAsync(idUsuario);
    }

    public async Task<int> CambiarContrasenaAsync(int idUsuario, string password)
    {
        var hash = _authService.HashPassword(password);

        return await _repository.CambiarContrasenaAsync(idUsuario, hash);
    }

    public async Task<ValidacionDesactivacionDto> ValidarDesactivacionAsync(int idUsuario)
    {
        return await _repository.ValidarDesactivacionAsync(idUsuario);
    }
}