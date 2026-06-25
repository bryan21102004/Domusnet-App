using DomusNet.API.DTOs;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly Services.Interfaces.IUsuariosService _service;

    public UsuariosController(Services.Interfaces.IUsuariosService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var data = await _service.ListarAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var data = await _service.BuscarAsync(id);
        if (data == null)
            return NotFound(new { mensaje = "Usuario no encontrado" });
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] UsuarioCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);
        return result.Resultado switch
        {
            1 => Ok(new { mensaje = "Usuario creado correctamente", id = result.IdGenerado }),
            -1 => BadRequest(new { mensaje = "Ese correo ya está registrado con un usuario activo. Usá otro correo o reactivá el usuario existente." }),
            -2 => BadRequest(new { mensaje = "Rol invalido" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] UsuarioUpdateDto dto)
    {
        var result = await _service.EditarAsync(id, dto);
        return result switch
        {
            1 => Ok(new { mensaje = "Usuario actualizado correctamente" }),
            0 => NotFound(new { mensaje = "Usuario no encontrado" }),
            -1 => BadRequest(new { mensaje = "Ese correo ya está registrado con otro usuario activo." }),
            -2 => BadRequest(new { mensaje = "Rol invalido" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var result = await _service.EliminarAsync(id);
        return result switch
        {
            1 => Ok(new { mensaje = "Usuario desactivado correctamente" }),
            0 => NotFound(new { mensaje = "Usuario no encontrado" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPut("{id}/contrasena")]
    public async Task<IActionResult> CambiarContrasena(int id, [FromBody] CambiarContrasenaDto dto)
    {
        var result = await _service.CambiarContrasenaAsync(id, dto.Password);
        return result switch
        {
            1 => Ok(new { mensaje = "Contrasena actualizada" }),
            0 => NotFound(new { mensaje = "Usuario no encontrado" }),
            _ => StatusCode(500, new { mensaje = "Error desconocido" })
        };
    }

    [HttpPut("{id}/reactivar")]
    public async Task<IActionResult> Reactivar(int id)
    {
        var usuario = await _service.BuscarAsync(id);
        if (usuario == null)
            return NotFound(new { mensaje = "Usuario no encontrado" });

        if (usuario.Activo)
            return BadRequest(new { mensaje = "El usuario ya está activo" });

        var result = await _service.EditarAsync(id, new UsuarioUpdateDto
        {
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Telefono = usuario.Telefono,
            IdRol = usuario.IdRol,
            Activo = true
        });

        return result switch
        {
            1 => Ok(new { mensaje = "Usuario reactivado correctamente" }),
            -1 => BadRequest(new { mensaje = "Ya existe otro usuario activo con ese correo" }),
            _ => StatusCode(500, new { mensaje = "Error al reactivar usuario" })
        };
    }

    [HttpGet("{id}/validar-desactivacion")]
    public async Task<IActionResult> ValidarDesactivacion(int id)
    {
        var data = await _service.ValidarDesactivacionAsync(id);

        var detalles = string.IsNullOrWhiteSpace(data.Detalles)
            ? []
            : data.Detalles.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        return Ok(new
        {
            tieneActividadPendiente = data.TieneActividadPendiente == 1,
            detalles
        });
    }
}
