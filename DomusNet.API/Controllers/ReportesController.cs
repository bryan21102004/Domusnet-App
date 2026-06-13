using DomusNet.API.Models;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly ReportesService _reportesService;

    public ReportesController(ReportesService reportesService)
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