using DomusNet.API.Models;
using DomusNet.API.Services.Interfaces;
using DomusNet.API.Services.ReglasNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstalacionesController : ControllerBase
{
    private readonly IInstalacionService _instalacionService;
    private readonly IEvidenciaInstalacionService _evidenciaService;
    private readonly ValidacionesGenerales _v = new();

    public InstalacionesController(
        IInstalacionService instalacionService,
        IEvidenciaInstalacionService evidenciaService)
    {
        _instalacionService = instalacionService;
        _evidenciaService = evidenciaService;
    }

    [HttpGet("tecnicos")]
    [Authorize(Roles = "Administrador,Vendedor")]
    public async Task<IActionResult> ListarTecnicos()
    {
        var tecnicos = await _instalacionService.ListarTecnicosActivosAsync();
        return Ok(tecnicos);
    }

    [HttpPost("programar")]
    [Authorize(Roles = "Administrador,Vendedor")]
    public async Task<IActionResult> ProgramarInstalacion([FromBody] ProgramarInstalacionRequest request)
    {
        try
        {
            _v.ValidarId(request.IdTecnicoAsignado, "Técnico asignado");
            _v.ValidarId(request.IdVendedorPrograma, "Vendedor");
            _v.ValidarFechaFutura(request.FechaProgramada, "Fecha programada");
            _v.ValidarLongitudMinima(request.UbicacionInstalacion?.Trim(), "Ubicación de instalación", 5);
            _v.ValidarLongitudMaxima(request.NotasVendedor?.Trim(), "Notas del vendedor", 500);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }

        if (request.UbicacionInstalacion != null)
            request.UbicacionInstalacion = request.UbicacionInstalacion.Trim();
        if (request.NotasVendedor != null)
            request.NotasVendedor = request.NotasVendedor.Trim();

        var resultado = await _instalacionService.ProgramarInstalacionAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpGet("tecnico/{idTecnico}")]
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> ListarPorTecnico(int idTecnico)
    {
        var instalaciones = await _instalacionService.ListarPorTecnicoAsync(idTecnico);
        return Ok(instalaciones);
    }

    [HttpPost("completar")]
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> CompletarInstalacion([FromBody] CompletarInstalacionRequest request)
    {
        var resultado = await _instalacionService.CompletarInstalacionAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Vendedor")]
    public async Task<IActionResult> ListarTodas()
    {
        var instalaciones = await _instalacionService.ListarTodasAsync();
        return Ok(instalaciones);
    }

    [HttpPost("subir-evidencia")]
    [Authorize(Roles = "Administrador,Tecnico")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> SubirEvidencia([FromForm] IFormFile archivo)
    {
        var resultado = await _evidenciaService.SubirEvidenciaAsync(archivo);

        if (!resultado.Exitoso)
        {
            return BadRequest(new
            {
                mensaje = resultado.Mensaje
            });
        }

        return Ok(new
        {
            ruta = resultado.Ruta,
            mensaje = resultado.Mensaje
        });
    }
}