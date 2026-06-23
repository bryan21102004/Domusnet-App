using DomusNet.API.Models;

namespace DomusNet.API.Repositories.Interfaces;

public interface IIngresosRepository
{
    Task<IEnumerable<ClienteIngresoResponse>> ListarClientesParaIngresoAsync();

    Task<ResultadoIngresoResponse?> RegistrarIngresoClienteAsync(RegistrarIngresoRequest request);

    Task<IEnumerable<IngresoResponse>> ListarIngresosAsync();
}