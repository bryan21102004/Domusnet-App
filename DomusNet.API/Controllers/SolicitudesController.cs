using DomusNet.API.DTOs;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Vendedor")]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudesService _service;
  

    public SolicitudesController(ISolicitudesService service)
    {
        _service = service;  
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] SolicitudCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);

        if (result.Resultado != 1)
        {
            return StatusCode(500, new
            {
                mensaje = result.Mensaje ?? "Error al registrar solicitud"
            });
        }
        return Ok(new
        {
            mensaje = result.Mensaje ?? "Solicitud registrada correctamente",
            idSolicitud = result.IdGenerado
          
        });
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? estado = null)
    {
        var data = await _service.ListarAsync(estado);
        return Ok(data);
    }

    [HttpPut("{id}/atender")]
    public async Task<IActionResult> Atender(int id, [FromBody] AtenderSolicitudDto dto)
    {
        var result = await _service.AtenderAsync(id, dto);

        return result switch
        {
            1 => Ok(new { mensaje = "Solicitud marcada como atendida" }),
            0 => NotFound(new { mensaje = "Solicitud no encontrada" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPost("{idSolicitud}/convertir-cliente")]
    public async Task<IActionResult> ConvertirSolicitudEnCliente(
        int idSolicitud,
        [FromBody] ConvertirSolicitudClienteDto dto)
    {
        var resultado = await _service.ConvertirSolicitudEnClienteAsync(
            idSolicitud,
            dto.IdVendedor,
            dto.Notas
        );

        if (resultado.Resultado == 1)
        {
            return Ok(new
            {
                mensaje = resultado.Mensaje ?? "Solicitud convertida en cliente correctamente",
                idCliente = resultado.IdGenerado
            });
        }

        return BadRequest(new
        {
            mensaje = resultado.Mensaje ?? "No se pudo convertir la solicitud en cliente",
            codigo = resultado.Resultado
        });
    }
}