using DomusNet.API.Models;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]

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
    [Authorize(Roles = "Administrador")]

    public async Task<IActionResult> GuardarConfiguracionDistribucion(
        [FromBody] GuardarConfiguracionDistribucionRequest request)
    {
        try
        {
            var resultado = await _reportesService.GuardarConfiguracionDistribucionAsync(request);

            if (resultado.Resultado == 1)
            {
                return Ok(resultado);
            }

            return BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = "Error guardando la configuración de distribución.",
                error = ex.Message,
                detalle = ex.InnerException?.Message,
                stack = ex.StackTrace
            });
        }
    }

    [HttpPost("generar-ingresos")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GenerarReporteIngresos(
        [FromBody] GenerarReporteIngresosRequest request)
    {
        try
        {
            var resultado = await _reportesService.GenerarReporteIngresosAsync(request);

            if (resultado.Resultado == 1)
            {
                return Ok(resultado);
            }

            return BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = "Error generando el reporte de ingresos.",
                error = ex.Message,
                detalle = ex.InnerException?.Message,
                stack = ex.StackTrace
            });
        }
    }

    [HttpGet("ingresos-generados/{idIngresoMensual}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerDetalleReporteIngreso(int idIngresoMensual)
    {
        try
        {
            var reporte = await _reportesService.ObtenerDetalleReporteIngresoAsync(idIngresoMensual);

            if (reporte == null)
            {
                return NotFound(new
                {
                    mensaje = $"No se encontró el reporte de ingresos #{idIngresoMensual}."
                });
            }

            return Ok(reporte);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = "Error consultando el detalle del reporte.",
                idIngresoMensual,
                error = ex.Message,
                detalle = ex.InnerException?.Message,
                stack = ex.StackTrace
            });
        }
    }

    [HttpGet("ingresos-generados/{idIngresoMensual}/pdf")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DescargarReporteIngresoPdf(int idIngresoMensual)
    {
        try
        {
            var resultado = await _reportePdfService.GenerarReporteIngresosPdfAsync(idIngresoMensual);

            if (resultado == null)
            {
                return NotFound(new
                {
                    mensaje = $"No se encontró el reporte de ingresos #{idIngresoMensual}."
                });
            }

            return File(
                resultado.Archivo,
                "application/pdf",
                resultado.NombreArchivo
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = "Error generando el PDF del reporte.",
                idIngresoMensual,
                error = ex.Message,
                detalle = ex.InnerException?.Message,
                stack = ex.StackTrace
            });
        }
    }
 
    [HttpGet("trabajadores")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarTrabajadores()
    {
        try
        {
            var trabajadores = await _reportesService.ListarTrabajadoresAsync();
            return Ok(trabajadores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = "Error listando trabajadores para distribución.",
                error = ex.Message,
                detalle = ex.InnerException?.Message,
                stack = ex.StackTrace
            });
        }
    }
}