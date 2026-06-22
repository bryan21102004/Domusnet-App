using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Data.Models;
using DomusNet.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DomusNet.API.Services;

public class AuthService : Interfaces.IAuthService
{
    private readonly DomusNetDBContext _context;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public AuthService(DomusNetDBContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public async Task<UsuarioLoginDto?> ValidateUserAsync(string correo, string password)
    {
        using var connection = _context.CreateConnection();
        var usuario = await connection.QueryFirstOrDefaultAsync<UsuarioLoginDto>(
            "buscarUsuarioLogin",
            new { Correo = correo },
            commandType: CommandType.StoredProcedure);

        if (usuario == null) return null;

        var resultado = _passwordHasher.VerifyHashedPassword(
            new Usuario(), usuario.Contrasena, password);

        if (resultado != PasswordVerificationResult.Success)
            return null;

        return usuario;
    }

    public string GenerateJwtToken(UsuarioLoginDto usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Role, usuario.NombreRol),
            new Claim("nombre", usuario.Nombre),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(int idUsuario)
    {
        var refreshToken = Guid.NewGuid().ToString("N");
        await ActualizarTokenAsync(idUsuario, refreshToken);
        return refreshToken;
    }

    public async Task ActualizarTokenAsync(int idUsuario, string refreshToken)
    {
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "modificarToken",
            new { IdUsuario = idUsuario, RefreshToken = refreshToken },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int?> VerificarRefreshTokenAsync(int idUsuario, string refreshToken)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<int?>(
            "verificarTokenR",
            new { IdUsuario = idUsuario, RefreshToken = refreshToken },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(string newToken, string newRefresh)?> RefreshTokenAsync(
        int idUsuario, string refreshToken, UsuarioLoginDto usuario)
    {
        var rol = await VerificarRefreshTokenAsync(idUsuario, refreshToken);
        if (rol == null) return null;

        var newJwt = GenerateJwtToken(usuario);
        var newRefresh = Guid.NewGuid().ToString("N");
        await ActualizarTokenAsync(idUsuario, newRefresh);

        return (newJwt, newRefresh);
    }

    public async Task LogoutAsync(int idUsuario)
    {
        await ActualizarTokenAsync(idUsuario, string.Empty);
    }

    public async Task<UsuarioLoginDto?> GetUsuarioAsync(int idUsuario)
    {
        using var connection = _context.CreateConnection();
        var usuario = await connection.QueryFirstOrDefaultAsync<UsuarioResponseDto>(
            "buscarUsuario",
            new { IdUsuario = idUsuario },
            commandType: CommandType.StoredProcedure);

        if (usuario == null) return null;

        return new UsuarioLoginDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Telefono = usuario.Telefono,
            IdRol = usuario.IdRol,
            NombreRol = usuario.NombreRol,
            Activo = usuario.Activo
        };
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new Usuario(), password);
    }
}
