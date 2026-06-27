using System.IdentityModel.Tokens.Jwt;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionesService _service;

    public NotificacionesController(INotificacionesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] bool soloNoLeidas = false)
    {
        var idUsuario = ObtenerIdUsuarioToken();

        var data = await _service.ListarAsync(idUsuario, soloNoLeidas);

        return Ok(data);
    }

    [HttpGet("no-leidas/count")]
    public async Task<IActionResult> ContarNoLeidas()
    {
        var idUsuario = ObtenerIdUsuarioToken();

        var total = await _service.ContarNoLeidasAsync(idUsuario);

        return Ok(new { totalNoLeidas = total });
    }

    [HttpPut("{id}/leida")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        var idUsuario = ObtenerIdUsuarioToken();

        var result = await _service.MarcarLeidaAsync(id, idUsuario);

        return result switch
        {
            1 => Ok(new { mensaje = "Notificación marcada como leída" }),
            0 => NotFound(new { mensaje = "Notificación no encontrada" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPut("leer-todas")]
    public async Task<IActionResult> MarcarTodasLeidas()
    {
        var idUsuario = ObtenerIdUsuarioToken();

        var total = await _service.MarcarTodasLeidasAsync(idUsuario);

        return Ok(new
        {
            mensaje = "Notificaciones marcadas como leídas",
            total
        });
    }

    private int ObtenerIdUsuarioToken()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claim == null || !int.TryParse(claim.Value, out var idUsuario))
        {
            throw new UnauthorizedAccessException("Token inválido.");
        }

        return idUsuario;
    }
}