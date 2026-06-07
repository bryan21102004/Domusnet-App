namespace DomusNet.API.DTOs;

public class SpResultDto
{
    public int Resultado { get; set; }
    public int IdGenerado { get; set; }
}

public class UsuarioLoginDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
    public bool Activo { get; set; }
    public string NombreRol { get; set; } = string.Empty;
}

public class UsuarioResponseDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class UsuarioCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
}

public class UsuarioUpdateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
    public bool Activo { get; set; } = true;
}

public class CambiarContrasenaDto
{
    public string Password { get; set; } = string.Empty;
}
