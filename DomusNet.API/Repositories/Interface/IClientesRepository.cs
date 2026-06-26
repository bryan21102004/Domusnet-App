using DomusNet.API.DTOs;

namespace DomusNet.API.Repositories.Interfaces;

public interface IClientesRepository
{
    Task<IEnumerable<dynamic>> ListarAsync(string? estadoPago = null);

    Task<dynamic?> BuscarAsync(int idCliente);

    Task<SpResultDto> CrearAsync(ClienteCreateDto dto);

    Task<int> EditarAsync(int idCliente, ClienteUpdateDto dto);

    Task<SpResultDto> AsignarPaqueteAsync(AsignarPaqueteDto dto);
}