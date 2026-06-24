using System.Net.Http.Json;
using System.Text.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class AuthFrontendService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AuthFrontendService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (loginResponse == null)
        {
            return null;
        }

        await _js.InvokeVoidAsync("localStorage.setItem", "token", loginResponse.Token);
        await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", loginResponse.RefreshToken);
        await _js.InvokeVoidAsync("localStorage.setItem", "usuarioId", loginResponse.UsuarioId.ToString());
        await _js.InvokeVoidAsync("localStorage.setItem", "rol", loginResponse.Rol);
        await _js.InvokeVoidAsync("localStorage.setItem", "nombre", loginResponse.Nombre);

        return loginResponse;
    }

    public async Task LogoutAsync()
    {
        var usuarioIdTexto = await _js.InvokeAsync<string?>("localStorage.getItem", "usuarioId");

        if (int.TryParse(usuarioIdTexto, out var usuarioId))
        {
            await _http.PostAsync($"api/Auth/logout/{usuarioId}", null);
        }

        await _js.InvokeVoidAsync("localStorage.removeItem", "token");
        await _js.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
        await _js.InvokeVoidAsync("localStorage.removeItem", "usuarioId");
        await _js.InvokeVoidAsync("localStorage.removeItem", "rol");
        await _js.InvokeVoidAsync("localStorage.removeItem", "nombre");
    }

    public async Task<bool> EstaLogueadoAsync()
    {
        var token = await ObtenerTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task<string?> ObtenerTokenAsync()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", "token");
    }

    public async Task<string?> ObtenerRolAsync()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", "rol");
    }

    public async Task<string?> ObtenerNombreAsync()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", "nombre");
    }

    public async Task<int> ObtenerUsuarioIdAsync()
    {
        var valor = await _js.InvokeAsync<string?>("localStorage.getItem", "usuarioId");

        return int.TryParse(valor, out var idUsuario)
            ? idUsuario
            : 0;
    }
}