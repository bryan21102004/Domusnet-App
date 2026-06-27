using System.Net.Http.Headers;
using System.Net.Http.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class ClientesVendedorService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public ClientesVendedorService(HttpClient http, IJSRuntime js)
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

    public async Task<List<ClienteResponse>> ListarClientesAsync(string? estadoPago = null)
    {
        await AgregarTokenAsync();

        var url = string.IsNullOrWhiteSpace(estadoPago)
            ? "api/Clientes"
            : $"api/Clientes?estadoPago={estadoPago}";

        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new List<ClienteResponse>();

        var clientes = await response.Content.ReadFromJsonAsync<List<ClienteResponse>>();
        return clientes ?? new List<ClienteResponse>();
    }
}
