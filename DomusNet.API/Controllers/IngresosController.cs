using DomusNet.API.Models;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class IngresosController : ControllerBase
{
    private readonly Services.Interfaces.IIngresosService _ingresosService;

    public IngresosController(Services.Interfaces.IIngresosService ingresosService)
    {
        _ingresosService = ingresosService;
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> ListarClientesParaIngreso()
    {
        var clientes = await _ingresosService.ListarClientesParaIngresoAsync();
        return Ok(clientes);
    }

    [HttpPost("registrar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegistrarIngreso([FromBody] RegistrarIngresoRequest request)
    {
        var resultado = await _ingresosService.RegistrarIngresoClienteAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> ListarIngresos()
    {
        var ingresos = await _ingresosService.ListarIngresosAsync();
        return Ok(ingresos);
    }
}