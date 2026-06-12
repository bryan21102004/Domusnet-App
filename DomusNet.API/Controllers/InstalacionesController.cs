using Microsoft.AspNetCore.Mvc;
using DomusNet.API.Models;
using DomusNet.API.Services;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstalacionesController : ControllerBase
{
    private readonly InstalacionService _instalacionService;

    public InstalacionesController(InstalacionService instalacionService)
    {
        _instalacionService = instalacionService;
    }

    [HttpGet("tecnicos")]
    public async Task<IActionResult> ListarTecnicos()
    {
        var tecnicos = await _instalacionService.ListarTecnicosActivosAsync();
        return Ok(tecnicos);
    }

    [HttpPost("programar")]
    public async Task<IActionResult> ProgramarInstalacion([FromBody] ProgramarInstalacionRequest request)
    {
        var resultado = await _instalacionService.ProgramarInstalacionAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpGet("tecnico/{idTecnico}")]
    public async Task<IActionResult> ListarPorTecnico(int idTecnico)
    {
        var instalaciones = await _instalacionService.ListarPorTecnicoAsync(idTecnico);
        return Ok(instalaciones);
    }

    [HttpPost("completar")]
    public async Task<IActionResult> CompletarInstalacion([FromBody] CompletarInstalacionRequest request)
    {
        var resultado = await _instalacionService.CompletarInstalacionAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }
}