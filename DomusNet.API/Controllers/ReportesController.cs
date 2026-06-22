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

    public ReportesController(IReportesService reportesService)
    {
        _reportesService = reportesService;
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
}