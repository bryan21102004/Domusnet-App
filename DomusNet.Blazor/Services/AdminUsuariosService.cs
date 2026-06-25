using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DomusNet.Blazor.Models;
using Microsoft.JSInterop;

namespace DomusNet.Blazor.Services;

public class AdminUsuariosService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AdminUsuariosService(HttpClient http, IJSRuntime js)
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

    public async Task<List<UsuarioResponse>> ListarAsync()
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync("api/Usuarios");

        if (!response.IsSuccessStatusCode)
            return [];

        var usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioResponse>>(JsonOptions);
        return usuarios ?? [];
    }

    public async Task<UsuarioResponse?> BuscarAsync(int idUsuario)
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync($"api/Usuarios/{idUsuario}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UsuarioResponse>(JsonOptions);
    }

    public async Task<OperacionUsuarioResponse> CrearAsync(UsuarioCreateRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PostAsJsonAsync("api/Usuarios", request);
        return await LeerRespuestaAsync(response);
    }

    public async Task<OperacionUsuarioResponse> EditarAsync(int idUsuario, UsuarioUpdateRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PutAsJsonAsync($"api/Usuarios/{idUsuario}", request);
        return await LeerRespuestaAsync(response);
    }

    public async Task<OperacionUsuarioResponse> EliminarAsync(int idUsuario)
    {
        await AgregarTokenAsync();

        var response = await _http.DeleteAsync($"api/Usuarios/{idUsuario}");
        return await LeerRespuestaAsync(response);
    }

    public async Task<OperacionUsuarioResponse> CambiarContrasenaAsync(int idUsuario, CambiarContrasenaRequest request)
    {
        await AgregarTokenAsync();

        var response = await _http.PutAsJsonAsync($"api/Usuarios/{idUsuario}/contrasena", request);
        return await LeerRespuestaAsync(response);
    }

    public async Task<OperacionUsuarioResponse> ReactivarAsync(int idUsuario)
    {
        await AgregarTokenAsync();

        var response = await _http.PutAsync($"api/Usuarios/{idUsuario}/reactivar", null);
        return await LeerRespuestaAsync(response);
    }

    public async Task<ValidacionDesactivacionResponse> ValidarDesactivacionAsync(int idUsuario)
    {
        await AgregarTokenAsync();

        var response = await _http.GetAsync($"api/Usuarios/{idUsuario}/validar-desactivacion");

        if (!response.IsSuccessStatusCode)
        {
            return new ValidacionDesactivacionResponse();
        }

        var validacion = await response.Content.ReadFromJsonAsync<ValidacionDesactivacionResponse>(JsonOptions);
        return validacion ?? new ValidacionDesactivacionResponse();
    }

    private static async Task<OperacionUsuarioResponse> LeerRespuestaAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            return new OperacionUsuarioResponse
            {
                Exito = response.IsSuccessStatusCode,
                Mensaje = response.IsSuccessStatusCode
                    ? "Operación completada."
                    : "No se pudo completar la operación."
            };
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;

            var mensaje = ObtenerMensajeJson(root);

            var idGenerado = 0;
            foreach (var prop in root.EnumerateObject())
            {
                if (prop.Name.Equals("id", StringComparison.OrdinalIgnoreCase) &&
                    prop.Value.TryGetInt32(out var id))
                {
                    idGenerado = id;
                    break;
                }
            }

            return new OperacionUsuarioResponse
            {
                Exito = response.IsSuccessStatusCode,
                Mensaje = string.IsNullOrWhiteSpace(mensaje)
                    ? (response.IsSuccessStatusCode ? "Operación completada." : "Error en la operación.")
                    : mensaje,
                IdGenerado = idGenerado
            };
        }
        catch
        {
            return new OperacionUsuarioResponse
            {
                Exito = response.IsSuccessStatusCode,
                Mensaje = response.IsSuccessStatusCode
                    ? "Operación completada."
                    : body
            };
        }
    }

    private static string? ObtenerMensajeJson(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
            return null;

        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Name.Equals("mensaje", StringComparison.OrdinalIgnoreCase) ||
                prop.Name.Equals("title", StringComparison.OrdinalIgnoreCase) ||
                prop.Name.Equals("detail", StringComparison.OrdinalIgnoreCase))
            {
                return prop.Value.GetString();
            }
        }

        return null;
    }
}
