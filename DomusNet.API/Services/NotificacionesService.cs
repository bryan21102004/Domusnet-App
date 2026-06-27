using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class NotificacionesService : INotificacionesService
{
    private readonly INotificacionesRepository _repository;

    public NotificacionesService(INotificacionesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(int idUsuario, bool soloNoLeidas = false)
    {
        return await _repository.ListarAsync(idUsuario, soloNoLeidas);
    }

    public async Task<int> ContarNoLeidasAsync(int idUsuario)
    {
        return await _repository.ContarNoLeidasAsync(idUsuario);
    }

    public async Task<int> MarcarLeidaAsync(int idNotificacion, int idUsuario)
    {
        return await _repository.MarcarLeidaAsync(idNotificacion, idUsuario);
    }

    public async Task<int> MarcarTodasLeidasAsync(int idUsuario)
    {
        return await _repository.MarcarTodasLeidasAsync(idUsuario);
    }
}