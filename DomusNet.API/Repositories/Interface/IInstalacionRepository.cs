using DomusNet.API.Models;

namespace DomusNet.API.Repositories.Interfaces;

public interface IInstalacionRepository
{
    Task<IEnumerable<TecnicoActivo>> ListarTecnicosActivosAsync();

    Task<ResultadoOperacion?> ProgramarInstalacionAsync(ProgramarInstalacionRequest request);

    Task<IEnumerable<InstalacionTecnicoResponse>> ListarPorTecnicoAsync(int idTecnico);

    Task<ResultadoOperacion?> CompletarInstalacionAsync(CompletarInstalacionRequest request);

    Task<IEnumerable<InstalacionGeneralResponse>> ListarTodasAsync();
}