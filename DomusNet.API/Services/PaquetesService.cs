using DomusNet.API.Data.Models;
using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class PaquetesService : IPaquetesService
{
    private readonly IPaquetesRepository _repository;

    public PaquetesService(IPaquetesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaqueteServicio>> ListarAsync(bool soloActivos = false)
    {
        return await _repository.ListarAsync(soloActivos);
    }

    public async Task<PaqueteServicio?> BuscarAsync(int idPaquete)
    {
        return await _repository.BuscarAsync(idPaquete);
    }

    public async Task<SpResultDto> CrearAsync(PaqueteCreateDto dto)
    {
        return await _repository.CrearAsync(dto);
    }

    public async Task<int> EditarAsync(int idPaquete, PaqueteUpdateDto dto)
    {
        return await _repository.EditarAsync(idPaquete, dto);
    }
}