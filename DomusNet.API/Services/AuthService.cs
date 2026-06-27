using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomusNet.API.Data.Models;
using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DomusNet.API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public AuthService(IAuthRepository repository, IConfiguration config)
    {
        _repository = repository;
        _config = config;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public async Task<UsuarioLoginDto?> ValidateUserAsync(string correo, string password)
    {
        var usuario = await _repository.BuscarUsuarioLoginAsync(correo);

        if (usuario == null)
        {
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            new Usuario(),
            usuario.Contrasena,
            password);

        if (resultado != PasswordVerificationResult.Success)
        {
            return null;
        }

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

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

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
        await _repository.ActualizarTokenAsync(idUsuario, refreshToken);
    }

    public async Task<int?> VerificarRefreshTokenAsync(int idUsuario, string refreshToken)
    {
        return await _repository.VerificarRefreshTokenAsync(idUsuario, refreshToken);
    }

    public async Task<(string newToken, string newRefresh)?> RefreshTokenAsync(
        int idUsuario,
        string refreshToken,
        UsuarioLoginDto usuario)
    {
        var rol = await VerificarRefreshTokenAsync(idUsuario, refreshToken);

        if (rol == null)
        {
            return null;
        }

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
        var usuario = await _repository.BuscarUsuarioAsync(idUsuario);

        if (usuario == null)
        {
            return null;
        }

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