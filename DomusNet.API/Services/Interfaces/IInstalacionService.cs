using System.Collections.Generic;
using System.Threading.Tasks;
using DomusNet.API.Models;

namespace DomusNet.API.Services.Interfaces;

public interface IInstalacionService
{
    Task<IEnumerable<TecnicoActivo>> ListarTecnicosActivosAsync();
    Task<ResultadoOperacion> ProgramarInstalacionAsync(ProgramarInstalacionRequest request);
    Task<IEnumerable<InstalacionTecnicoResponse>> ListarPorTecnicoAsync(int idTecnico);
    Task<ResultadoOperacion> CompletarInstalacionAsync(CompletarInstalacionRequest request);
    Task<IEnumerable<InstalacionGeneralResponse>> ListarTodasAsync();
}
