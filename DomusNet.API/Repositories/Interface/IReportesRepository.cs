using DomusNet.API.Models;

namespace DomusNet.API.Repositories.Interfaces;

public interface IReportesRepository
{
    Task<ResultadoOperacion?> GuardarConfiguracionDistribucionAsync(
        GuardarConfiguracionDistribucionRequest request);

    Task<ResultadoOperacion?> GenerarReporteIngresosAsync(
        GenerarReporteIngresosRequest request);

    Task<DetalleReporteIngresoResponse?> ObtenerDetalleReporteIngresoAsync(
        int idIngresoMensual);
}