using DomusNet.API.Models;
using DomusNet.Shared.DTOs;

namespace DomusNet.API.Services.Interfaces;

public interface IReportesService
{
    Task<IEnumerable<TrabajadorUsuarioResponse>> ListarTrabajadoresAsync();
    Task<ResultadoOperacion> GuardarConfiguracionDistribucionAsync(
        GuardarConfiguracionDistribucionRequest request);

    Task<ResultadoOperacion> GenerarReporteIngresosAsync(
        GenerarReporteIngresosRequest request);

    Task<DetalleReporteIngresoResponse?> ObtenerDetalleReporteIngresoAsync(
        int idIngresoMensual);
}