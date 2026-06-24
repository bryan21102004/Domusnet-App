using System.Net.Http.Headers;
using System.Net.Http.Json;
using DomusNet.Blazor.Models;

namespace DomusNet.Blazor.Services;

public class ReportesService
{
    private readonly HttpClient _http;
    private readonly AuthFrontendService _authService;

    public ReportesService(HttpClient http, AuthFrontendService authService)
    {
        _http = http;
        _authService = authService;
    }

    private async Task AgregarTokenAsync()
    {
        var token = await _authService.ObtenerTokenAsync();

        _http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<ResultadoOperacion?> GuardarConfiguracionAsync(
        GuardarConfiguracionDistribucionRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PostAsJsonAsync(
            "api/Reportes/configuracion-distribucion",
            request
        );

        return await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
    }

    public async Task<ResultadoOperacion?> GenerarReporteAsync(
        GenerarReporteIngresosRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PostAsJsonAsync(
            "api/Reportes/generar-ingresos",
            request
        );

        return await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
    }

    public async Task<DetalleReporteIngresoResponse?> ObtenerDetalleAsync(int idIngresoMensual)
    {
        await AgregarTokenAsync();

        return await _http.GetFromJsonAsync<DetalleReporteIngresoResponse>(
            $"api/Reportes/ingresos-generados/{idIngresoMensual}"
        );
    }
    public async Task<byte[]?> DescargarPdfAsync(int idIngresoMensual)
{
    await AgregarTokenAsync();

    var response = await _http.GetAsync(
        $"api/Reportes/ingresos-generados/{idIngresoMensual}/pdf"
    );

    if (!response.IsSuccessStatusCode)
    {
        return null;
    }

    return await response.Content.ReadAsByteArrayAsync();
}
}