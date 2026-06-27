using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DomusNet.API.Tests.Integration;

public class ApiProtectedEndpointsMoreTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiProtectedEndpointsMoreTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("GET", "/api/Clientes")]
    [InlineData("GET", "/api/Clientes/1")]
    [InlineData("POST", "/api/Clientes")]
    [InlineData("PUT", "/api/Clientes/1")]
    [InlineData("POST", "/api/Clientes/asignar-paquete")]
    [InlineData("GET", "/api/Usuarios")]
    [InlineData("GET", "/api/Usuarios/1")]
    [InlineData("POST", "/api/Usuarios")]
    [InlineData("PUT", "/api/Usuarios/1")]
    [InlineData("DELETE", "/api/Usuarios/1")]
    [InlineData("GET", "/api/Ingresos")]
    [InlineData("GET", "/api/Ingresos/clientes")]
    [InlineData("POST", "/api/Ingresos/registrar")]
    [InlineData("GET", "/api/Reportes/ingresos-generados/1")]
    [InlineData("GET", "/api/Notificaciones")]
    [InlineData("GET", "/api/Tickets")]
    [InlineData("GET", "/api/Tickets/1")]
    [InlineData("PUT", "/api/Tickets/1/estado")]
    public async Task EndpointsProtegidos_SinToken_DebenRetornar401(string metodo, string url)
    {
        
        var request = new HttpRequestMessage(new HttpMethod(metodo), url);

        
        var response = await _client.SendAsync(request);

        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}