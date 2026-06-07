using DomusNet.API.DTOs;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class IngresosController : ControllerBase
{
    private readonly IngresosService _service;

    public IngresosController(IngresosService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var data = await _service.ListarAsync(desde, hasta);
        return Ok(data);
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen([FromQuery] int? mes, [FromQuery] int? anio)
    {
        var data = await _service.ResumenAsync(mes, anio);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var data = await _service.BuscarAsync(id);
        if (data == null)
            return NotFound(new { mensaje = "Ingreso no encontrado" });
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] IngresoCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);
        return result.Resultado switch
        {
            1 => Ok(new { mensaje = "Ingreso registrado correctamente", id = result.IdGenerado }),
            -1 => BadRequest(new { mensaje = "El monto debe ser mayor a cero" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }
}
