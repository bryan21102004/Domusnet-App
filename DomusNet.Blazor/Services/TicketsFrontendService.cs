using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class TicketsFrontendService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TicketsFrontendService(HttpClient http, IJSRuntime js)
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

    public async Task<ResultadoTicketResponse?> ReportarAveriaAsync(ReportarAveriaRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Tickets/reportar-cliente", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ResultadoTicketResponse>(JsonOptions);
    }

    public async Task<List<TicketResponse>> ListarTicketsAsync(string? estado = null)
    {
        await AgregarTokenAsync();

        var url = string.IsNullOrWhiteSpace(estado)
            ? "api/Tickets"
            : $"api/Tickets?estado={Uri.EscapeDataString(estado)}";

        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return new List<TicketResponse>();
        }

        var tickets = await response.Content.ReadFromJsonAsync<List<TicketResponse>>(JsonOptions);
        return tickets ?? new List<TicketResponse>();
    }

    public async Task<List<TicketResponse>> ListarGlobalesAsync()
    {
        var response = await _http.GetAsync("api/Tickets/globales");

        if (!response.IsSuccessStatusCode)
        {
            return new List<TicketResponse>();
        }

        var tickets = await response.Content.ReadFromJsonAsync<List<TicketResponse>>(JsonOptions);
        return tickets ?? new List<TicketResponse>();
    }

    public async Task<List<HistorialTicketResponse>> ObtenerHistorialAsync(int idTicket)
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync($"api/Tickets/{idTicket}/historial");

        if (!response.IsSuccessStatusCode)
        {
            return new List<HistorialTicketResponse>();
        }

        var historial = await response.Content.ReadFromJsonAsync<List<HistorialTicketResponse>>(JsonOptions);
        return historial ?? new List<HistorialTicketResponse>();
    }

    public async Task<ResultadoTicketResponse?> CrearTicketAsync(TicketCreateRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PostAsJsonAsync("api/Tickets", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ResultadoTicketResponse>(JsonOptions);
    }

    public async Task<bool> ActualizarEstadoAsync(int idTicket, ActualizarEstadoTicketRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PutAsJsonAsync($"api/Tickets/{idTicket}/estado", request);
        return response.IsSuccessStatusCode;
    }
}
