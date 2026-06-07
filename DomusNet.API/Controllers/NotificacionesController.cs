using System.IdentityModel.Tokens.Jwt;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly NotificacionesService _service;

    public NotificacionesController(NotificacionesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] bool soloNoLeidas = false)
    {
        var idUsuario = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var data = await _service.ListarAsync(idUsuario, soloNoLeidas);
        return Ok(data);
    }

    [HttpGet("no-leidas/count")]
    public async Task<IActionResult> ContarNoLeidas()
    {
        var idUsuario = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var total = await _service.ContarNoLeidasAsync(idUsuario);
        return Ok(new { totalNoLeidas = total });
    }

    [HttpPut("{id}/leida")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        var idUsuario = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var result = await _service.MarcarLeidaAsync(id, idUsuario);
        return result switch
        {
            1 => Ok(new { mensaje = "Notificacion marcada como leida" }),
            0 => NotFound(new { mensaje = "Notificacion no encontrada" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPut("leer-todas")]
    public async Task<IActionResult> MarcarTodasLeidas()
    {
        var idUsuario = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var total = await _service.MarcarTodasLeidasAsync(idUsuario);
        return Ok(new { mensaje = "Notificaciones marcadas como leidas", total });
    }
}
