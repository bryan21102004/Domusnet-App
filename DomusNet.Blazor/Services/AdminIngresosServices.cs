using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class AdminIngresosService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AdminIngresosService(HttpClient http, IJSRuntime js)
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

    public async Task<List<ClienteIngresoResponse>> ListarClientesParaIngresoAsync()
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync("api/Ingresos/clientes");

        if (!response.IsSuccessStatusCode)
        {
            return new List<ClienteIngresoResponse>();
        }

        var clientes = await response.Content.ReadFromJsonAsync<List<ClienteIngresoResponse>>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return clientes ?? new List<ClienteIngresoResponse>();
    }

    public async Task<List<IngresoResponse>> ListarIngresosAsync()
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync("api/Ingresos");

        if (!response.IsSuccessStatusCode)
        {
            return new List<IngresoResponse>();
        }

        var ingresos = await response.Content.ReadFromJsonAsync<List<IngresoResponse>>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return ingresos ?? new List<IngresoResponse>();
    }

    public async Task<ResultadoIngresoResponse?> RegistrarIngresoAsync(RegistrarIngresoRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PostAsJsonAsync("api/Ingresos/registrar", request);

        var resultado = await response.Content.ReadFromJsonAsync<ResultadoIngresoResponse>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return resultado;
    }

   public async Task<int> ObtenerIdUsuarioAsync()
{
    var usuarioIdTexto = await _js.InvokeAsync<string>("localStorage.getItem", "usuarioId");

    if (int.TryParse(usuarioIdTexto, out var usuarioId))
    {
        return usuarioId;
    }

    return 0;
}
}