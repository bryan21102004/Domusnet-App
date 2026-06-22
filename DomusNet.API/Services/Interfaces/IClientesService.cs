using System.Collections.Generic;
using System.Threading.Tasks;
using DomusNet.API.DTOs;

namespace DomusNet.API.Services.Interfaces;

public interface IClientesService
{
    Task<IEnumerable<dynamic>> ListarAsync(string? estadoPago = null);
    Task<dynamic?> BuscarAsync(int idCliente);
    Task<SpResultDto> CrearAsync(ClienteCreateDto dto);
    Task<int> EditarAsync(int idCliente, ClienteUpdateDto dto);
    Task<SpResultDto> AsignarPaqueteAsync(AsignarPaqueteDto dto);
}
