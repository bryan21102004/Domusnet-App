using System.Net.Http.Headers;
using System.Net.Http.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class PaquetesFrontendService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public PaquetesFrontendService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    private async Task AgregarTokenAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");
        _http.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrWhiteSpace(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<PaqueteResponse>> ListarActivosAsync()
    {
        var response = await _http.GetAsync("api/Paquetes?soloActivos=true");
        if (!response.IsSuccessStatusCode)
            return [];
        return await response.Content.ReadFromJsonAsync<List<PaqueteResponse>>() ?? [];
    }

    public async Task<List<PaqueteResponse>> ListarTodosAsync()
    {
        var response = await _http.GetAsync("api/Paquetes?soloActivos=false");
        if (!response.IsSuccessStatusCode)
            return [];
        return await response.Content.ReadFromJsonAsync<List<PaqueteResponse>>() ?? [];
    }

    public async Task<PaqueteResponse?> BuscarAsync(int idPaquete)
    {
        return await _http.GetFromJsonAsync<PaqueteResponse>($"api/Paquetes/{idPaquete}");
    }

    public async Task<OperacionPaqueteResponse> CrearAsync(PaqueteCreateRequest request)
    {
        await AgregarTokenAsync();
        var response = await _http.PostAsJsonAsync("api/Paquetes", request);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<ApiMensajeResponse>();
            return new OperacionPaqueteResponse { Exito = true, Mensaje = body?.Mensaje ?? "Paquete creado correctamente.", IdGenerado = body?.Id };
        }

        var error = await LeerMensajeErrorAsync(response);
        return new OperacionPaqueteResponse { Exito = false, Mensaje = error };
    }

    public async Task<OperacionPaqueteResponse> EditarAsync(int idPaquete, PaqueteUpdateRequest request)
    {
        await AgregarTokenAsync();
        var response = await _http.PutAsJsonAsync($"api/Paquetes/{idPaquete}", request);
        if (response.IsSuccessStatusCode)
            return new OperacionPaqueteResponse { Exito = true, Mensaje = "Paquete actualizado correctamente." };

        var error = await LeerMensajeErrorAsync(response);
        return new OperacionPaqueteResponse { Exito = false, Mensaje = error };
    }

    private static async Task<string> LeerMensajeErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<ApiMensajeResponse>();
            return body?.Mensaje ?? "Error al procesar la solicitud.";
        }
        catch
        {
            return "Error al procesar la solicitud.";
        }
    }

    private class ApiMensajeResponse
    {
        public string? Mensaje { get; set; }
        public int? Id { get; set; }
    }
}