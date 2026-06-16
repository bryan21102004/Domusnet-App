using System.Net.Http.Json;
using DomusNet.Blazor.Models;

namespace DomusNet.Blazor.Services;

public class PaquetesFrontendService
{
    private readonly HttpClient _http;

    public PaquetesFrontendService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PaqueteResponse>> ListarActivosAsync()
    {
        var response = await _http.GetAsync("api/Paquetes?soloActivos=true");

        if (!response.IsSuccessStatusCode)
        {
            return new List<PaqueteResponse>();
        }

        var paquetes = await response.Content.ReadFromJsonAsync<List<PaqueteResponse>>();

        return paquetes ?? new List<PaqueteResponse>();
    }

    public async Task<PaqueteResponse?> BuscarAsync(int idPaquete)
    {
        return await _http.GetFromJsonAsync<PaqueteResponse>($"api/Paquetes/{idPaquete}");
    }
}