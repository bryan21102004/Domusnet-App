namespace DomusNet.Blazor.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
    public int UsuarioId { get; set; }
    public string Rol { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

   