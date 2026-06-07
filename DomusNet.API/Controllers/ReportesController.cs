using System.IdentityModel.Tokens.Jwt;
using DomusNet.API.DTOs;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class ReportesController : ControllerBase
{
    private readonly ReportesService _service;

    public ReportesController(ReportesService service)
    {
        _service = service;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var data = await _service.ObtenerDashboardAsync();
        return Ok(data);
    }

    [HttpGet("ingresos")]
    public async Task<IActionResult> ReporteIngresos([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var data = await _service.ReporteIngresosAsync(desde, hasta);
        return Ok(data);
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> ReporteClientes([FromQuery] string? estadoPago = null)
    {
        var data = await _service.ReporteClientesAsync(estadoPago);
        return Ok(data);
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> ReporteTickets(
        [FromQuery] string? estado = null,
        [FromQuery] string? tipo = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        var data = await _service.ReporteTicketsAsync(estado, tipo, desde, hasta);
        return Ok(data);
    }

    [HttpGet("historial")]
    public async Task<IActionResult> HistorialGenerados()
    {
        var data = await _service.ListarGeneradosAsync();
        return Ok(data);
    }

    [HttpPost("guardar")]
    public async Task<IActionResult> Guardar([FromBody] GuardarReporteDto dto)
    {
        var idUsuario = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var result = await _service.GuardarReporteAsync(dto, idUsuario);
        return result.Resultado == 1
            ? Ok(new { mensaje = "Reporte guardado en historial", id = result.IdGenerado })
            : StatusCode(500, new { mensaje = "Error al guardar reporte" });
    }
}
