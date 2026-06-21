using Microsoft.AspNetCore.Mvc;
using DomusNet.API.Models;
using DomusNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Authorize(Roles = "Administrador,Tecnico")]
    public async Task<IActionResult> ListarTecnicos()
    {
        var tecnicos = await _instalacionService.ListarTecnicosActivosAsync();
        return Ok(tecnicos);
    }

    [HttpPost("programar")]
      [Authorize(Roles = "Administrador,Vendedor")]
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
       [Authorize(Roles = "Administrador,Tecnico")]
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
[HttpGet]
[Authorize(Roles = "Administrador,Vendedor")]
public async Task<IActionResult> ListarTodas()
{
    var instalaciones = await _instalacionService.ListarTodasAsync();
    return Ok(instalaciones);
}


[HttpPost("subir-evidencia")]
[RequestSizeLimit(10_000_000)]
public async Task<IActionResult> SubirEvidencia([FromForm] IFormFile archivo)
{
    if (archivo == null || archivo.Length == 0)
    {
        return BadRequest(new { mensaje = "Debe seleccionar una imagen." });
    }

    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    var extension = Path.GetExtension(archivo.FileName).ToLower();

    if (!extensionesPermitidas.Contains(extension))
    {
        return BadRequest(new { mensaje = "Solo se permiten imágenes JPG, JPEG, PNG o WEBP." });
    }

    var carpeta = Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot",
        "evidencias",
        "instalaciones"
    );

    if (!Directory.Exists(carpeta))
    {
        Directory.CreateDirectory(carpeta);
    }

    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
    var rutaFisica = Path.Combine(carpeta, nombreArchivo);

    using (var stream = new FileStream(rutaFisica, FileMode.Create))
    {
        await archivo.CopyToAsync(stream);
    }

    var rutaPublica = $"/evidencias/instalaciones/{nombreArchivo}";

    return Ok(new
    {
        ruta = rutaPublica,
        mensaje = "Imagen subida correctamente."
    });
}
}