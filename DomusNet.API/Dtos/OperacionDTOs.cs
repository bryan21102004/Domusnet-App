namespace DomusNet.API.DTOs;

public class PaqueteCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Velocidad { get; set; }
    public decimal Precio { get; set; }
    public decimal? PorcentajeDistribucion { get; set; }
}

public class PaqueteUpdateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Velocidad { get; set; }
    public decimal Precio { get; set; }
    public decimal? PorcentajeDistribucion { get; set; }
    public string Estado { get; set; } = "Activo";
}

public class ClienteCreateDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public int IdVendedor { get; set; }
    public int? IdSolicitudOrigen { get; set; }
}

public class ClienteUpdateDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string EstadoPago { get; set; } = "AlDia";
}

public class AsignarPaqueteDto
{
    public int IdCliente { get; set; }
    public int IdPaquete { get; set; }
    public int IdVendedor { get; set; }
}

public class TicketCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Tipo { get; set; } = "Averia";
    public string Prioridad { get; set; } = "Media";
    public int? IdCliente { get; set; }
    public int IdCreadoPor { get; set; }
    public int? IdAsignadoA { get; set; }
    public bool EsGlobal { get; set; }
}

public class ActualizarEstadoTicketDto
{
    public string EstadoNuevo { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public string? Comentario { get; set; }
}

public class SolicitudCreateDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public int? IdPaquete { get; set; }
}

public class SolicitudResultDto
{
    public int Resultado { get; set; }
    public int IdGenerado { get; set; }
    public int IdClienteExterno { get; set; }
     public string? Mensaje { get; set; }
}

public class AtenderSolicitudDto
{
    public int IdVendedorAsignado { get; set; }
    public string? Notas { get; set; }
}
