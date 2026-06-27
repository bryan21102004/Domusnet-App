using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DomusNet.API.Tests.Integration;

public class ApiAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiAuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/Clientes")]
    [InlineData("/api/Ingresos")]
    [InlineData("/api/Reportes/ingresos-generados/1")]
    [InlineData("/api/Usuarios")]
    [InlineData("/api/Notificaciones")]
    [InlineData("/api/Tickets")]
    public async Task EndpointsProtegidos_SinToken_DebenResponderUnauthorized(string url)
    {
        
        var response = await _client.GetAsync(url);

        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}