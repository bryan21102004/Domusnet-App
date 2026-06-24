using DomusNet.API.Models;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class ReportesController : ControllerBase
{
    private readonly IReportesService _reportesService;
    private readonly IReportePdfService _reportePdfService;

    
    public ReportesController(
    IReportesService reportesService,
    IReportePdfService reportePdfService)
{
    _reportesService = reportesService;
    _reportePdfService = reportePdfService;
}

    [HttpPost("configuracion-distribucion")]
    public async Task<IActionResult> GuardarConfiguracionDistribucion(
        [FromBody] GuardarConfiguracionDistribucionRequest request)
    {
        var resultado = await _reportesService.GuardarConfiguracionDistribucionAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpPost("generar-ingresos")]
    public async Task<IActionResult> GenerarReporteIngresos(
        [FromBody] GenerarReporteIngresosRequest request)
    {
        var resultado = await _reportesService.GenerarReporteIngresosAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpGet("ingresos-generados/{idIngresoMensual}")]
    public async Task<IActionResult> ObtenerDetalleReporteIngreso(int idIngresoMensual)
    {
        var reporte = await _reportesService.ObtenerDetalleReporteIngresoAsync(idIngresoMensual);

        if (reporte == null)
        {
            return NotFound(new
            {
                mensaje = "No se encontró el reporte de ingresos."
            });
        }

        return Ok(reporte);
    }

    [HttpGet("ingresos-generados/{idIngresoMensual}/pdf")]
public async Task<IActionResult> DescargarReporteIngresoPdf(int idIngresoMensual)
{
    var resultado = await _reportePdfService.GenerarReporteIngresosPdfAsync(idIngresoMensual);

    if (resultado == null)
    {
        return NotFound(new
        {
            mensaje = "No se encontró el reporte de ingresos."
        });
    }

    return File(
        resultado.Archivo,
        "application/pdf",
        resultado.NombreArchivo
    );
}
}