namespace DomusNet.API.Repositories.Interfaces;

public interface INotificacionesRepository
{
    Task<IEnumerable<dynamic>> ListarAsync(int idUsuario, bool soloNoLeidas = false);

    Task<int> ContarNoLeidasAsync(int idUsuario);

    Task<int> MarcarLeidaAsync(int idNotificacion, int idUsuario);

    Task<int> MarcarTodasLeidasAsync(int idUsuario);
}