using DomusNet.API.Models;
using Microsoft.AspNetCore.Http;

namespace DomusNet.API.Services.Interfaces;

public interface IEvidenciaInstalacionService
{
    Task<SubirEvidenciaResultado> SubirEvidenciaAsync(IFormFile archivo);
}