using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class ClientesService : IClientesService
{
    private readonly IClientesRepository _repository;

    public ClientesService(IClientesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estadoPago = null)
    {
        return await _repository.ListarAsync(estadoPago);
    }

    public async Task<dynamic?> BuscarAsync(int idCliente)
    {
        return await _repository.BuscarAsync(idCliente);
    }

    public async Task<SpResultDto> CrearAsync(ClienteCreateDto dto)
    {
        return await _repository.CrearAsync(dto);
    }

    public async Task<int> EditarAsync(int idCliente, ClienteUpdateDto dto)
    {
        return await _repository.EditarAsync(idCliente, dto);
    }

    public async Task<SpResultDto> AsignarPaqueteAsync(AsignarPaqueteDto dto)
    {
        return await _repository.AsignarPaqueteAsync(dto);
    }
}