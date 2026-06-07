using DomusNet.API.DTOs;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly SolicitudesService _service;
    private readonly IConfiguration _config;

    public SolicitudesController(SolicitudesService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] SolicitudCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);

        if (result.Resultado != 1)
            return StatusCode(500, new { mensaje = "Error al registrar solicitud" });

        var mensajeWhatsApp = Uri.EscapeDataString(
            $"Hola, solicito informacion sobre un paquete de internet. " +
            $"Nombre: {dto.NombreCompleto}, Telefono: {dto.Telefono}");

        var numeroWhatsApp = _config["WhatsApp:NumeroVendedor"] ?? "50600000000";
        var enlaceWhatsApp = $"https://wa.me/{numeroWhatsApp}?text={mensajeWhatsApp}";

        return Ok(new
        {
            mensaje = "Solicitud registrada correctamente",
            idSolicitud = result.IdGenerado,
            enlaceWhatsApp
        });
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Vendedor")]
    public async Task<IActionResult> Listar([FromQuery] string? estado = null)
    {
        var data = await _service.ListarAsync(estado);
        return Ok(data);
    }

    [HttpPut("{id}/atender")]
    [Authorize(Roles = "Administrador,Vendedor")]
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
}
