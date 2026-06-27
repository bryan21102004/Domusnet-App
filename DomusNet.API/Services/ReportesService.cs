using DomusNet.API.Models;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;
using DomusNet.Shared.DTOs;
using System.Data;
using Dapper;

namespace DomusNet.API.Services;

public class ReportesService : IReportesService
{
    private readonly IReportesRepository _repository;

    public ReportesService(IReportesRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultadoOperacion> GuardarConfiguracionDistribucionAsync(
        GuardarConfiguracionDistribucionRequest request)
    {
        var resultado = await _repository.GuardarConfiguracionDistribucionAsync(request);

        return resultado ?? new ResultadoOperacion
        {
            Resultado = -1,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }

    public async Task<ResultadoOperacion> GenerarReporteIngresosAsync(
        GenerarReporteIngresosRequest request)
    {
        var resultado = await _repository.GenerarReporteIngresosAsync(request);

        return resultado ?? new ResultadoOperacion
        {
            Resultado = -1,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }

    public async Task<DetalleReporteIngresoResponse?> ObtenerDetalleReporteIngresoAsync(
        int idIngresoMensual)
    {
        return await _repository.ObtenerDetalleReporteIngresoAsync(idIngresoMensual);
    }

 public async Task<IEnumerable<TrabajadorUsuarioResponse>> ListarTrabajadoresAsync()
    {
        return await _repository.ListarTrabajadoresAsync();
    }

}