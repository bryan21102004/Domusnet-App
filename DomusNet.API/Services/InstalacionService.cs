using DomusNet.API.Models;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class InstalacionService : IInstalacionService
{
    private readonly IInstalacionRepository _repository;

    public InstalacionService(IInstalacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TecnicoActivo>> ListarTecnicosActivosAsync()
    {
        return await _repository.ListarTecnicosActivosAsync();
    }

    public async Task<ResultadoOperacion> ProgramarInstalacionAsync(ProgramarInstalacionRequest request)
    {
        var resultado = await _repository.ProgramarInstalacionAsync(request);

        return resultado ?? new ResultadoOperacion
        {
            Resultado = -1,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }

    public async Task<IEnumerable<InstalacionTecnicoResponse>> ListarPorTecnicoAsync(int idTecnico)
    {
        return await _repository.ListarPorTecnicoAsync(idTecnico);
    }

    public async Task<ResultadoOperacion> CompletarInstalacionAsync(CompletarInstalacionRequest request)
    {
        var resultado = await _repository.CompletarInstalacionAsync(request);

        return resultado ?? new ResultadoOperacion
        {
            Resultado = -1,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }

    public async Task<IEnumerable<InstalacionGeneralResponse>> ListarTodasAsync()
    {
        return await _repository.ListarTodasAsync();
    }
}