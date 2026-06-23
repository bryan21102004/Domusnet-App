using DomusNet.API.DTOs;

namespace DomusNet.API.Repositories.Interfaces;

public interface ISolicitudesRepository
{
    Task<SolicitudResultDto> CrearAsync(SolicitudCreateDto dto);

    Task<IEnumerable<dynamic>> ListarAsync(string? estado = null);

    Task<int> AtenderAsync(int idSolicitud, AtenderSolicitudDto dto);

    Task<SpResultDto?> ConvertirSolicitudEnClienteAsync(
        int idSolicitud,
        int idVendedor,
        string? notas
    );
}