using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomusNet.API.Services.Interfaces;

public interface INotificacionesService
{
    Task<IEnumerable<dynamic>> ListarAsync(int idUsuario, bool soloNoLeidas = false);
    Task<int> ContarNoLeidasAsync(int idUsuario);
    Task<int> MarcarLeidaAsync(int idNotificacion, int idUsuario);
    Task<int> MarcarTodasLeidasAsync(int idUsuario);
}
