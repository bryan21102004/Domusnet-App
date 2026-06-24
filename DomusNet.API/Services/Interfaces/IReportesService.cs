using DomusNet.API.Models;

namespace DomusNet.API.Services.Interfaces;

public interface IReportesService
{
    Task<ResultadoOperacion> GuardarConfiguracionDistribucionAsync(
        GuardarConfiguracionDistribucionRequest request);

    Task<ResultadoOperacion> GenerarReporteIngresosAsync(
        GenerarReporteIngresosRequest request);

    Task<DetalleReporteIngresoResponse?> ObtenerDetalleReporteIngresoAsync(
        int idIngresoMensual);
}