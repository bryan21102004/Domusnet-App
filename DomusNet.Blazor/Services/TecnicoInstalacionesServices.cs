using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DomusNet.Blazor.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class TecnicoInstalacionesService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public TecnicoInstalacionesService(HttpClient http, IJSRuntime js)
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

    public async Task<List<InstalacionTecnicoResponse>> ListarPorTecnicoAsync(int idTecnico)
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync($"api/Instalaciones/tecnico/{idTecnico}");

        if (!response.IsSuccessStatusCode)
        {
            return new List<InstalacionTecnicoResponse>();
        }

        var instalaciones = await response.Content.ReadFromJsonAsync<List<InstalacionTecnicoResponse>>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return instalaciones ?? new List<InstalacionTecnicoResponse>();
    }

    public async Task<ResultadoOperacion?> CompletarInstalacionAsync(CompletarInstalacionRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PostAsJsonAsync("api/Instalaciones/completar", request);

        if (!response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ResultadoOperacion>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        return await response.Content.ReadFromJsonAsync<ResultadoOperacion>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
public async Task<string?> SubirEvidenciaAsync(IBrowserFile archivo)
{
    await AgregarTokenAsync();

    using var contenido = new MultipartFormDataContent();

    await using var stream = archivo.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);

    var archivoContent = new StreamContent(stream);
    archivoContent.Headers.ContentType = new MediaTypeHeaderValue(archivo.ContentType);

    contenido.Add(archivoContent, "archivo", archivo.Name);

    var response = await _http.PostAsync("api/Instalaciones/subir-evidencia", contenido);

    if (!response.IsSuccessStatusCode)
    {
        return null;
    }

    var resultado = await response.Content.ReadFromJsonAsync<SubirEvidenciaResponse>(
        new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    return resultado?.Ruta;
}



}