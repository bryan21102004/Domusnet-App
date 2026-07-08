using System.Net.Http.Headers;
using System.Net.Http.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;
namespace DomusNet.Blazor.Services;

public class InstalacionesVendedorService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public InstalacionesVendedorService(HttpClient http, IJSRuntime js)
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

    public async Task<List<InstalacionGeneralResponse>> ListarTodasAsync()
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync("api/Instalaciones");

        if (!response.IsSuccessStatusCode)
        {
            return new List<InstalacionGeneralResponse>();
        }

        var instalaciones = await response.Content.ReadFromJsonAsync<List<InstalacionGeneralResponse>>();

        return instalaciones ?? new List<InstalacionGeneralResponse>();
    }

    public async Task<List<TecnicoResponse>> ListarTecnicosActivosAsync()
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync("api/Instalaciones/tecnicos");

        if (!response.IsSuccessStatusCode)
        {
            return new List<TecnicoResponse>();
        }

        var tecnicos = await response.Content.ReadFromJsonAsync<List<TecnicoResponse>>();

        return tecnicos ?? new List<TecnicoResponse>();
    }

    public async Task<ResultadoOperacion?> ProgramarInstalacionAsync(ProgramarInstalacionRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PostAsJsonAsync("api/Instalaciones/programar", request);

        if (!response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
        }

        return await response.Content.ReadFromJsonAsync<ResultadoOperacion>();
    }
public async Task<ResultadoOperacion?> ConvertirSolicitudEnClienteAsync(
  int idSolicitud,
    int idVendedor,
    string? notas,
    string? numeroContrato) 
{
    await AgregarTokenAsync();
    var request = new
    {
        idVendedor = idVendedor,
        notas = notas,
        numeroContrato = numeroContrato 
    };
    var response = await _http.PostAsJsonAsync(
        $"api/solicitudes/{idSolicitud}/convertir-cliente",
        request
    );

    var contenido = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode)
    {
        var resultadoApi = await response.Content.ReadFromJsonAsync<ConvertirClienteResponse>();

        return new ResultadoOperacion
        {
            Resultado = 1,
            Mensaje = resultadoApi?.Mensaje ?? "Solicitud convertida en cliente correctamente."
        };
    }

    return new ResultadoOperacion
    {
        Resultado = -1,
        Mensaje = contenido
    };
}
}