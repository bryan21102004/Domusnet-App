using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomusNet.API.Services;

public class TicketsService
{
    private readonly DomusNetDBContext _context;
    private readonly EmailService _emailService;
    private readonly ILogger<TicketsService> _logger;
    private readonly int _idUsuarioSistema;

    public TicketsService(
        DomusNetDBContext context,
        EmailService emailService,
        ILogger<TicketsService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;

        // Si no existe la configuración, usa 1 por defecto.
        _idUsuarioSistema = configuration.GetValue<int?>("Tickets:IdUsuarioSistema") ?? 1;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estado = null, int? idAsignadoA = null)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync(
            "listarTickets",
            new { Estado = estado, IdAsignadoA = idAsignadoA },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dynamic?> BuscarAsync(int idTicket)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync(
            "buscarTicket",
            new { IdTicket = idTicket },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> ListarHistorialAsync(int idTicket)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync(
            "listarHistorialTicket",
            new { IdTicket = idTicket },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> ListarGlobalesAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync(
            "listarTicketsGlobales",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResultDto> CrearAsync(TicketCreateDto dto)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryFirstAsync<SpResultDto>(
            "nuevoTicket",
            dto,
            commandType: CommandType.StoredProcedure);

        var tipo = dto.Tipo?.Trim() ?? "";

        var esTipoAveria =
            tipo.Equals("Averia", StringComparison.OrdinalIgnoreCase) ||
            tipo.Equals("Avería", StringComparison.OrdinalIgnoreCase);

        // Si es una avería global, notificar a los clientes activos por correo
        if (result.Resultado == 1 && dto.EsGlobal && esTipoAveria)
        {
            var titulo = string.IsNullOrWhiteSpace(dto.Titulo)
                ? "Avería general"
                : dto.Titulo.Trim();

            var descripcion = string.IsNullOrWhiteSpace(dto.Descripcion)
                ? "Se ha reportado una avería general en el servicio."
                : dto.Descripcion.Trim();

            _ = Task.Run(() => EnviarNotificacionesAveriaGlobalAsync(titulo, descripcion));
        }

        return result;
    }

    private async Task EnviarNotificacionesAveriaGlobalAsync(string titulo, string descripcion)
    {
        try
        {
            using var bgConnection = _context.CreateConnection();

            var clientesActivos = await bgConnection.QueryAsync<ClienteCorreoDto>(
                @"SELECT NombreCompleto, Correo
                  FROM Clientes
                  WHERE Activo = 1
                  AND Correo IS NOT NULL
                  AND LTRIM(RTRIM(Correo)) <> ''"
            );

            foreach (var cliente in clientesActivos)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(cliente.Correo))
                        continue;

                    var subject = $"[DomusNet] Aviso de Avería General: {titulo}";

                    var body =
                        $"Estimado/a {cliente.NombreCompleto},\n\n" +
                        $"Te informamos que se ha detectado una avería general que podría estar afectando tu servicio de Internet.\n\n" +
                        $"Detalle del reporte:\n" +
                        $"- {descripcion}\n\n" +
                        $"Nuestro personal técnico ya está al tanto y se encuentra trabajando para resolver el inconveniente lo antes posible.\n\n" +
                        $"Lamentamos los inconvenientes ocasionados.\n\n" +
                        $"Atentamente,\n" +
                        $"Soporte Técnico DomusNet";

                    await _emailService.SendEmailAsync(cliente.Correo, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo enviar correo al cliente {Correo}.", cliente.Correo);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificaciones masivas de avería global.");
        }
    }

    public async Task<SpResultDto> ReportarClienteAsync(ReportarAveriaClienteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NombreCompleto) ||
            string.IsNullOrWhiteSpace(dto.Telefono) ||
            string.IsNullOrWhiteSpace(dto.Direccion) ||
            string.IsNullOrWhiteSpace(dto.DescripcionProblema))
        {
            return new SpResultDto
            {
                Resultado = 0,
               
            };
        }

        using var connection = _context.CreateConnection();

        var telefono = dto.Telefono.Trim();

        // Buscar si hay un cliente activo con el teléfono dado
        var cliente = await connection.QueryFirstOrDefaultAsync<ClienteReporteDto>(
            @"SELECT IdCliente, NombreCompleto, Direccion
              FROM Clientes
              WHERE Telefono = @Telefono
              AND Activo = 1",
            new { Telefono = telefono }
        );

        int? idCliente = null;
        string nombreCliente = dto.NombreCompleto.Trim();
        string direccionCliente = dto.Direccion.Trim();

        if (cliente != null)
        {
            idCliente = cliente.IdCliente;

            if (!string.IsNullOrWhiteSpace(cliente.NombreCompleto))
                nombreCliente = cliente.NombreCompleto;

            if (!string.IsNullOrWhiteSpace(cliente.Direccion))
                direccionCliente = cliente.Direccion;
        }

        var titulo = $"Avería de Cliente - {nombreCliente}";

        var descripcionFormateada =
            $"Cliente: {nombreCliente}\n" +
            $"Teléfono: {telefono}\n" +
            $"Dirección: {direccionCliente}\n" +
            $"Detalle del problema: {dto.DescripcionProblema.Trim()}";

        var result = await connection.QueryFirstAsync<SpResultDto>(
            "nuevoTicket",
            new
            {
                Titulo = titulo,
                Descripcion = descripcionFormateada,
                Tipo = "Averia",
                Prioridad = "Alta",
                IdCliente = idCliente,
                IdCreadoPor = _idUsuarioSistema,
                IdAsignadoA = (int?)null,
                EsGlobal = false
            },
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<int> ActualizarEstadoAsync(int idTicket, ActualizarEstadoTicketDto dto)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstAsync<int>(
            "actualizarEstadoTicket",
            new
            {
                IdTicket = idTicket,
                dto.EstadoNuevo,
                dto.IdUsuario,
                dto.Comentario
            },
            commandType: CommandType.StoredProcedure);
    }

   
}