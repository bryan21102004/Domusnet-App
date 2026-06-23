using DomusNet.API.Data.Models;
using DomusNet.API.DTOs;

namespace DomusNet.API.Repositories.Interfaces;

public interface IPaquetesRepository
{
    Task<IEnumerable<PaqueteServicio>> ListarAsync(bool soloActivos = false);

    Task<PaqueteServicio?> BuscarAsync(int idPaquete);

    Task<SpResultDto> CrearAsync(PaqueteCreateDto dto);

    Task<int> EditarAsync(int idPaquete, PaqueteUpdateDto dto);
}