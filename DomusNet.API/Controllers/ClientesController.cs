using DomusNet.API.DTOs;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Vendedor")]
public class ClientesController : ControllerBase
{
    private readonly ClientesService _service;

    public ClientesController(ClientesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? estadoPago = null)
    {
        var data = await _service.ListarAsync(estadoPago);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var data = await _service.BuscarAsync(id);
        if (data == null)
            return NotFound(new { mensaje = "Cliente no encontrado" });
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ClienteCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);
        return result.Resultado switch
        {
            1 => Ok(new { mensaje = "Cliente creado correctamente", id = result.IdGenerado }),
            -1 => BadRequest(new { mensaje = "El telefono ya esta registrado" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] ClienteUpdateDto dto)
    {
        var result = await _service.EditarAsync(id, dto);
        return result switch
        {
            1 => Ok(new { mensaje = "Cliente actualizado correctamente" }),
            0 => NotFound(new { mensaje = "Cliente no encontrado" }),
            -1 => BadRequest(new { mensaje = "El telefono ya esta registrado" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPost("asignar-paquete")]
    public async Task<IActionResult> AsignarPaquete([FromBody] AsignarPaqueteDto dto)
    {
        var result = await _service.AsignarPaqueteAsync(dto);
        return result.Resultado switch
        {
            1 => Ok(new { mensaje = "Paquete asignado correctamente", id = result.IdGenerado }),
            -1 => BadRequest(new { mensaje = "El cliente ya tiene ese paquete activo" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }
}
