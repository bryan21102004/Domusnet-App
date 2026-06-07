using DomusNet.API.DTOs;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IConfiguration _config;

    public AuthController(AuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _authService.ValidateUserAsync(request.Correo, request.Password);
        if (usuario == null)
            return Unauthorized(new { error = "Credenciales invalidas" });

        var token = _authService.GenerateJwtToken(usuario);
        var refresh = await _authService.GenerateRefreshTokenAsync(usuario.IdUsuario);

        return Ok(new LoginResponse
        {
            Token = token,
            Expira = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
            UsuarioId = usuario.IdUsuario,
            Rol = usuario.NombreRol,
            RefreshToken = refresh,
            Nombre = usuario.Nombre
        });
    }

    [HttpPost("logout/{idUsuario}")]
    public async Task<IActionResult> Logout(int idUsuario)
    {
        await _authService.LogoutAsync(idUsuario);
        return Ok(new { mensaje = "Sesion cerrada correctamente" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var usuario = await _authService.GetUsuarioAsync(request.IdUsuario);
        if (usuario == null)
            return Unauthorized(new { error = "Usuario no encontrado" });

        var result = await _authService.RefreshTokenAsync(
            request.IdUsuario, request.RefreshToken, usuario);

        if (result == null)
            return Unauthorized(new { error = "Refresh Token invalido" });

        return Ok(new RefreshResponse
        {
            Token = result.Value.newToken,
            RefreshToken = result.Value.newRefresh
        });
    }
}
