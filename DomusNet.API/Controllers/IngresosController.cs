using DomusNet.API.Models;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DomusNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngresosController : ControllerBase
{
    private readonly IngresosService _ingresosService;

    public IngresosController(IngresosService ingresosService)
    {
        _ingresosService = ingresosService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarIngreso([FromBody] RegistrarIngresoRequest request)
    {
        var resultado = await _ingresosService.RegistrarIngresoAsync(request);

        if (resultado.Resultado == 1)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> ListarIngresos(
        [FromQuery] int? mes,
        [FromQuery] int? anio,
        [FromQuery] string? quincena,
        [FromQuery] string? estado)
    {
        var ingresos = await _ingresosService.ListarIngresosAsync(
            mes,
            anio,
            quincena,
            estado
        );

        return Ok(ingresos);
    }

   
}