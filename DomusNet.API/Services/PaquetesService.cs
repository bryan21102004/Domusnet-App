using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;
using DomusNet.API.Data.Models;

namespace DomusNet.API.Services;

public class PaquetesService
{
    private readonly DomusNetDBContext _context;

    public PaquetesService(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaqueteServicio>> ListarAsync(bool soloActivos = false)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PaqueteServicio>(
            "listarPaquetes",
            new { SoloActivos = soloActivos },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<PaqueteServicio?> BuscarAsync(int idPaquete)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<PaqueteServicio>(
            "buscarPaquete",
            new { IdPaquete = idPaquete },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearAsync(PaqueteCreateDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<SpResultDto>(
            "nuevoPaquete",
            dto,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EditarAsync(int idPaquete, PaqueteUpdateDto dto)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<int>(
            "editarPaquete",
            new
            {
                IdPaquete = idPaquete,
                dto.Nombre,
                dto.Descripcion,
                dto.Velocidad,
                dto.Precio,
                dto.PorcentajeDistribucion,
                dto.Estado
            },
            commandType: CommandType.StoredProcedure);
    }
}
