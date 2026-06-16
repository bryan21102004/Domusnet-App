using System.Net.Http.Headers;
using System.Net.Http.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class SolicitudesVendedorService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public SolicitudesVendedorService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    private async Task AgregarTokenAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");

        _http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<SolicitudServicioDto>> ListarSolicitudesAsync()
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync("api/Solicitudes");

        if (!response.IsSuccessStatusCode)
        {
            return new List<SolicitudServicioDto>();
        }

        var solicitudes = await response.Content.ReadFromJsonAsync<List<SolicitudServicioDto>>();

        return solicitudes ?? new List<SolicitudServicioDto>();
    }

    public async Task<ResultadoOperacion?> AtenderSolicitudAsync(int idSolicitud, AtenderSolicitudRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PutAsJsonAsync($"api/Solicitudes/{idSolicitud}/atender", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
            return error;
        }

        return await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
    }
}