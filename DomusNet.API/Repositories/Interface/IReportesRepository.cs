using DomusNet.API.Models;
using DomusNet.Shared.DTOs;
namespace DomusNet.API.Repositories.Interfaces;

public interface IReportesRepository
{
    Task<IEnumerable<TrabajadorUsuarioResponse>> ListarTrabajadoresAsync();
    Task<ResultadoOperacion?> GuardarConfiguracionDistribucionAsync(
        GuardarConfiguracionDistribucionRequest request);

    Task<ResultadoOperacion?> GenerarReporteIngresosAsync(
        GenerarReporteIngresosRequest request);

    Task<DetalleReporteIngresoResponse?> ObtenerDetalleReporteIngresoAsync(
        int idIngresoMensual);
}