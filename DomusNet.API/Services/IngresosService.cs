using DomusNet.API.Models;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class IngresosService : IIngresosService
{
    private readonly IIngresosRepository _repository;

    public IngresosService(IIngresosRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ClienteIngresoResponse>> ListarClientesParaIngresoAsync()
    {
        return await _repository.ListarClientesParaIngresoAsync();
    }

    public async Task<ResultadoIngresoResponse> RegistrarIngresoClienteAsync(RegistrarIngresoRequest request)
    {
        var resultado = await _repository.RegistrarIngresoClienteAsync(request);

        return resultado ?? new ResultadoIngresoResponse
        {
            Resultado = -1,
            IdGenerado = 0,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }

    public async Task<IEnumerable<IngresoResponse>> ListarIngresosAsync()
    {
        return await _repository.ListarIngresosAsync();
    }
}