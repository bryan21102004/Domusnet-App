namespace DomusNet.Blazor.Models;

public class ReportarAveriaRequest
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string DescripcionProblema { get; set; } = string.Empty;
}

public class TicketResponse
{
    public int IdTicket { get; set; }
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
    public string? Prioridad { get; set; }
    public int? IdCliente { get; set; }
    public string? NombreCliente { get; set; }
    public int IdCreadoPor { get; set; }
    public string? NombreCreador { get; set; }
    public int? IdAsignadoA { get; set; }
    public string? NombreAsignado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public bool EsGlobal { get; set; }
}

public class HistorialTicketResponse
{
    public int IdHistorial { get; set; }
    public string? EstadoAnterior { get; set; }
    public string? EstadoNuevo { get; set; }
    public string? Comentario { get; set; }
    public int IdUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public DateTime Fecha { get; set; }
}

public class TicketCreateRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Tipo { get; set; } = "Averia";
    public string Prioridad { get; set; } = "Alta";
    public int? IdCliente { get; set; }
    public int IdCreadoPor { get; set; }
    public int? IdAsignadoA { get; set; }
    public bool EsGlobal { get; set; }
}

public class ActualizarEstadoTicketRequest
{
    public string EstadoNuevo { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public string? Comentario { get; set; }
}

public class ResultadoTicketResponse
{
    public string? Mensaje { get; set; }
    public int Id { get; set; }
}

public class OperacionTicketResponse
{
    public bool Exitoso { get; set; }
    public string? Mensaje { get; set; }
    public int Id { get; set; }
}

public class ClienteVerificadoResponse
{
    public int IdCliente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}
