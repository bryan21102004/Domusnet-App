using DomusNet.API.DTOs;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaquetesController : ControllerBase
{
    private readonly IPaquetesService _service;

    public PaquetesController(IPaquetesService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar([FromQuery] bool soloActivos = false)
    {
        var data = await _service.ListarAsync(soloActivos);
        return Ok(data);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Buscar(int id)
    {
        var data = await _service.BuscarAsync(id);

        if (data == null)
        {
            return NotFound(new { mensaje = "Paquete no encontrado" });
        }

        return Ok(data);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] PaqueteCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);

        return result.Resultado == 1
            ? Ok(new { mensaje = "Paquete creado correctamente", id = result.IdGenerado })
            : StatusCode(500, new { mensaje = "Error al crear paquete" });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(int id, [FromBody] PaqueteUpdateDto dto)
    {
        var result = await _service.EditarAsync(id, dto);

        return result switch
        {
            1 => Ok(new { mensaje = "Paquete actualizado correctamente" }),
            0 => NotFound(new { mensaje = "Paquete no encontrado" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }
}