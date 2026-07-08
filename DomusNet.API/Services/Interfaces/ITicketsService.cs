using System.Collections.Generic;
using System.Threading.Tasks;
using DomusNet.API.DTOs;
using DomusNet.API.Models;

namespace DomusNet.API.Services.Interfaces;

public interface ITicketsService
{
    Task<IEnumerable<dynamic>> ListarAsync(string? estado = null, int? idAsignadoA = null);
    Task<dynamic?> BuscarAsync(int idTicket);
    Task<IEnumerable<dynamic>> ListarHistorialAsync(int idTicket);
    Task<IEnumerable<dynamic>> ListarGlobalesAsync();
    Task<SpResultDto> CrearAsync(TicketCreateDto dto);
    Task<SpResultDto> ReportarClienteAsync(ReportarAveriaClienteDto dto);
    Task<ClienteReporteDto?> VerificarClientePorTelefonoAsync(string telefono);
    Task<ClienteVerificadoResponse?> VerificarClientePorContratoAsync(string numeroContrato);
    Task<int> ActualizarEstadoAsync(int idTicket, ActualizarEstadoTicketDto dto);
}
