using System.Collections.Generic;
using System.Threading.Tasks;
using DomusNet.API.Models;

namespace DomusNet.API.Services.Interfaces;

public interface IIngresosService
{
    Task<IEnumerable<ClienteIngresoResponse>> ListarClientesParaIngresoAsync();
    Task<ResultadoIngresoResponse> RegistrarIngresoClienteAsync(RegistrarIngresoRequest request);
    Task<IEnumerable<IngresoResponse>> ListarIngresosAsync();
}
