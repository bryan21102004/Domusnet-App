using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class SolicitudesService : ISolicitudesService
{
    private readonly ISolicitudesRepository _repository;

    public SolicitudesService(ISolicitudesRepository repository)
    {
        _repository = repository;
    }

    public async Task<SolicitudResultDto> CrearAsync(SolicitudCreateDto dto)
    {
        return await _repository.CrearAsync(dto);
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estado = null)
    {
        return await _repository.ListarAsync(estado);
    }

    public async Task<int> AtenderAsync(int idSolicitud, AtenderSolicitudDto dto)
    {
        return await _repository.AtenderAsync(idSolicitud, dto);
    }

    public async Task<SpResultDto> ConvertirSolicitudEnClienteAsync(
        int idSolicitud,
        int idVendedor,
        string? notas,
        string? numeroContrato) 
    {
        var resultado = await _repository.ConvertirSolicitudEnClienteAsync(
            idSolicitud,
            idVendedor,
            notas,
            numeroContrato 
        );
        

        return resultado ?? new SpResultDto
        {
            Resultado = 0,
            IdGenerado = 0
        };
    }
}