using DomusNet.API.DTOs;
using DomusNet.API.Repositories.Interfaces;
using DomusNet.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomusNet.API.Services;

public class TicketsService : ITicketsService
{
    private readonly ITicketsRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<TicketsService> _logger;
    private readonly int _idUsuarioSistema;

    public TicketsService(
        ITicketsRepository repository,
        IEmailService emailService,
        ILogger<TicketsService> logger,
        IConfiguration configuration)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
        _idUsuarioSistema = configuration.GetValue<int?>("Tickets:IdUsuarioSistema") ?? 1;
    }

    public async Task<IEnumerable<dynamic>> ListarAsync(string? estado = null, int? idAsignadoA = null)
    {
        return await _repository.ListarAsync(estado, idAsignadoA);
    }

    public async Task<dynamic?> BuscarAsync(int idTicket)
    {
        return await _repository.BuscarAsync(idTicket);
    }

    public async Task<IEnumerable<dynamic>> ListarHistorialAsync(int idTicket)
    {
        return await _repository.ListarHistorialAsync(idTicket);
    }

    public async Task<IEnumerable<dynamic>> ListarGlobalesAsync()
    {
        return await _repository.ListarGlobalesAsync();
    }

    public async Task<SpResultDto> CrearAsync(TicketCreateDto dto)
    {
        var result = await _repository.CrearAsync(dto);

        var tipo = dto.Tipo?.Trim() ?? string.Empty;

        var esTipoAveria =
            tipo.Equals("Averia", StringComparison.OrdinalIgnoreCase) ||
            tipo.Equals("Avería", StringComparison.OrdinalIgnoreCase);

        if (result.Resultado == 1 && dto.EsGlobal && esTipoAveria)
        {
            var titulo = string.IsNullOrWhiteSpace(dto.Titulo)
                ? "Avería general"
                : dto.Titulo.Trim();

            var descripcion = string.IsNullOrWhiteSpace(dto.Descripcion)
                ? "Se ha reportado una avería general en el servicio."
                : dto.Descripcion.Trim();

            await EnviarNotificacionesAveriaGlobalAsync(titulo, descripcion);
        }

        return result;
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
                IdGenerado = 0
            };
        }

        var telefono = dto.Telefono.Trim();

        var cliente = await _repository.BuscarClienteActivoPorTelefonoAsync(telefono);

        int? idCliente = null;
        var nombreCliente = dto.NombreCompleto.Trim();
        var direccionCliente = dto.Direccion.Trim();

        if (cliente != null)
        {
            idCliente = cliente.IdCliente;

            if (!string.IsNullOrWhiteSpace(cliente.NombreCompleto))
            {
                nombreCliente = cliente.NombreCompleto;
            }

            if (!string.IsNullOrWhiteSpace(cliente.Direccion))
            {
                direccionCliente = cliente.Direccion;
            }
        }

        var titulo = $"Avería de Cliente - {nombreCliente}";

        var descripcionFormateada =
            $"Cliente: {nombreCliente}\n" +
            $"Teléfono: {telefono}\n" +
            $"Dirección: {direccionCliente}\n" +
            $"Detalle del problema: {dto.DescripcionProblema.Trim()}";

        return await _repository.CrearReporteClienteAsync(
            titulo,
            descripcionFormateada,
            idCliente,
            _idUsuarioSistema
        );
    }

    public async Task<int> ActualizarEstadoAsync(int idTicket, ActualizarEstadoTicketDto dto)
    {
        return await _repository.ActualizarEstadoAsync(idTicket, dto);
    }

    private async Task EnviarNotificacionesAveriaGlobalAsync(string titulo, string descripcion)
    {
        try
        {
            var clientesActivos = await _repository.ListarClientesActivosConCorreoAsync();

            foreach (var cliente in clientesActivos)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(cliente.Correo))
                    {
                        continue;
                    }

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

                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "No se pudo enviar correo al cliente {Correo}.",
                        cliente.Correo
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al enviar notificaciones masivas de avería global."
            );
        }
    }
}