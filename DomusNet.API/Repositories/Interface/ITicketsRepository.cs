using DomusNet.API.DTOs;

namespace DomusNet.API.Repositories.Interfaces;

public interface ITicketsRepository
{
    Task<IEnumerable<dynamic>> ListarAsync(string? estado = null, int? idAsignadoA = null);

    Task<dynamic?> BuscarAsync(int idTicket);

    Task<IEnumerable<dynamic>> ListarHistorialAsync(int idTicket);

    Task<IEnumerable<dynamic>> ListarGlobalesAsync();

    Task<SpResultDto> CrearAsync(TicketCreateDto dto);

    Task<IEnumerable<ClienteCorreoDto>> ListarClientesActivosConCorreoAsync();

    Task<ClienteReporteDto?> BuscarClienteActivoPorTelefonoAsync(string telefono);

    Task<SpResultDto> CrearReporteClienteAsync(
        string titulo,
        string descripcion,
        int? idCliente,
        int idUsuarioSistema
    );

    Task<int> ActualizarEstadoAsync(int idTicket, ActualizarEstadoTicketDto dto);
}