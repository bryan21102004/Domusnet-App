using DomusNet.API.DTOs;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketsService _service;

    public TicketsController(ITicketsService service)
    {
        _service = service;
    }

    [HttpGet("globales")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarGlobales()
    {
        var data = await _service.ListarGlobalesAsync();
        return Ok(data);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> Listar(
        [FromQuery] string? estado = null,
        [FromQuery] int? idAsignadoA = null)
    {
        var data = await _service.ListarAsync(estado, idAsignadoA);
        return Ok(data);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> Buscar(int id)
    {
        var data = await _service.BuscarAsync(id);

        if (data == null)
        {
            return NotFound(new { mensaje = "Ticket no encontrado" });
        }

        return Ok(data);
    }

    [HttpGet("{id}/historial")]
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> Historial(int id)
    {
        var data = await _service.ListarHistorialAsync(id);
        return Ok(data);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> Crear([FromBody] TicketCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);

        if (result.Resultado == 1)
        {
            return Ok(new
            {
                mensaje = "Ticket creado correctamente",
                id = result.IdGenerado
            });
        }

        return BadRequest(new
        {
            mensaje = "No se pudo crear el ticket"
        });
    }

    [HttpPost("reportar-cliente")]
    [AllowAnonymous]
    public async Task<IActionResult> ReportarCliente([FromBody] ReportarAveriaClienteDto dto)
    {
        var result = await _service.ReportarClienteAsync(dto);

        if (result.Resultado == 1)
        {
            return Ok(new
            {
                mensaje = "Reporte de avería creado correctamente",
                id = result.IdGenerado
            });
        }

        return BadRequest(new
        {
            mensaje = "No se pudo registrar el reporte de avería. Verifique que todos los campos estén completos."
        });
    }

    [HttpPut("{id}/estado")]
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> ActualizarEstado(
        int id,
        [FromBody] ActualizarEstadoTicketDto dto)
    {
        var result = await _service.ActualizarEstadoAsync(id, dto);

        return result switch
        {
            1 => Ok(new { mensaje = "Estado actualizado correctamente" }),
            0 => NotFound(new { mensaje = "Ticket no encontrado" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }
}