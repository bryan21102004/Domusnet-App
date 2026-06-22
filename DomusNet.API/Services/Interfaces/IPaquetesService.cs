using System.Collections.Generic;
using System.Threading.Tasks;
using DomusNet.API.DTOs;
using DomusNet.API.Data.Models;

namespace DomusNet.API.Services.Interfaces;

public interface IPaquetesService
{
    Task<IEnumerable<PaqueteServicio>> ListarAsync(bool soloActivos = false);
    Task<PaqueteServicio?> BuscarAsync(int idPaquete);
    Task<SpResultDto> CrearAsync(PaqueteCreateDto dto);
    Task<int> EditarAsync(int idPaquete, PaqueteUpdateDto dto);
}
