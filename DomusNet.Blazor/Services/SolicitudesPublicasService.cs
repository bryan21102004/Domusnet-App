using System.Net.Http.Json;
using DomusNet.Blazor.Models;

namespace DomusNet.Blazor.Services;

public class SolicitudesPublicasService
{
    private readonly HttpClient _http;

    public SolicitudesPublicasService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ResultadoOperacion?> CrearSolicitudAsync(SolicitudPaqueteRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Solicitudes", request);

        if (!response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
        }

        return await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
    }
}