using DomusNet.API.Models;
using DomusNet.API.Services.Interfaces;

namespace DomusNet.API.Services;

public class EvidenciaInstalacionService : IEvidenciaInstalacionService
{
    private readonly IWebHostEnvironment _environment;

    public EvidenciaInstalacionService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<SubirEvidenciaResultado> SubirEvidenciaAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return new SubirEvidenciaResultado
            {
                Exitoso = false,
                Mensaje = "Debe seleccionar una imagen."
            };
        }

        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(archivo.FileName).ToLower();

        if (!extensionesPermitidas.Contains(extension))
        {
            return new SubirEvidenciaResultado
            {
                Exitoso = false,
                Mensaje = "Solo se permiten imágenes JPG, JPEG, PNG o WEBP."
            };
        }

        var carpeta = Path.Combine(
            _environment.WebRootPath,
            "evidencias",
            "instalaciones"
        );

        if (!Directory.Exists(carpeta))
        {
            Directory.CreateDirectory(carpeta);
        }

        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(carpeta, nombreArchivo);

        await using var stream = new FileStream(rutaFisica, FileMode.Create);
        await archivo.CopyToAsync(stream);

        var rutaPublica = $"/evidencias/instalaciones/{nombreArchivo}";

        return new SubirEvidenciaResultado
        {
            Exitoso = true,
            Mensaje = "Imagen subida correctamente.",
            Ruta = rutaPublica
        };
    }
}