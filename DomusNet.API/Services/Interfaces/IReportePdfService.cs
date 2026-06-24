using DomusNet.API.Models;

namespace DomusNet.API.Services.Interfaces;

public interface IReportePdfService
{
    Task<ReportePdfResultado?> GenerarReporteIngresosPdfAsync(int idIngresoMensual);
}