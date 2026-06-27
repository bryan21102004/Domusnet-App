using System.Security.Claims;
using DomusNet.API.Controllers;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DomusNet.API.Tests.Controllers;

public class NotificacionesControllerTests
{
    private static NotificacionesController CrearControllerSinToken(INotificacionesService service)
    {
        var controller = new NotificacionesController(service);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        return controller;
    }

    [Fact]
    public async Task Listar_SinToken_DebeLanzarUnauthorizedAccessException()
    {
        
        var serviceMock = new Mock<INotificacionesService>();
        var controller = CrearControllerSinToken(serviceMock.Object);

        
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            controller.Listar()
        );
    }

    [Fact]
    public async Task ContarNoLeidas_SinToken_DebeLanzarUnauthorizedAccessException()
    {
        
        var serviceMock = new Mock<INotificacionesService>();
        var controller = CrearControllerSinToken(serviceMock.Object);

        
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            controller.ContarNoLeidas()
        );
    }
}