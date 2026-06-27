using System.Reflection;
using DomusNet.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DomusNet.API.Tests.Controllers;

public class ControllerMetadataTests
{
    [Fact]
    public void Controladores_DebenTener_ApiController_Y_RutaBase()
    {
        
        var controllers = new[]
        {
            typeof(AuthController),
            typeof(ClientesController),
            typeof(IngresosController),
            typeof(InstalacionesController),
            typeof(SolicitudesController),
            typeof(NotificacionesController),
            typeof(PaquetesController),
            typeof(ReportesController),
            typeof(TicketsController),
            typeof(UsuariosController)
        };

        foreach (var controller in controllers)
        {
            
            var apiController = controller.GetCustomAttribute<ApiControllerAttribute>();
            var route = controller.GetCustomAttribute<RouteAttribute>();

            
            Assert.NotNull(apiController);
            Assert.NotNull(route);
            Assert.Equal("api/[controller]", route!.Template);
        }
    }

    [Theory]
    [InlineData(typeof(ClientesController), "Administrador,Vendedor")]
    [InlineData(typeof(IngresosController), "Administrador")]
    [InlineData(typeof(ReportesController), "Administrador")]
    [InlineData(typeof(SolicitudesController), "Administrador,Vendedor")]
    [InlineData(typeof(UsuariosController), "Administrador")]
    public void ControladoresProtegidos_DebenTener_AuthorizeConRoles(Type controller, string rolesEsperados)
    {
        
        var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();

        
        Assert.NotNull(authorize);
        Assert.Equal(rolesEsperados, authorize!.Roles);
    }

    [Theory]
    [InlineData(typeof(SolicitudesController), nameof(SolicitudesController.Crear))]
    [InlineData(typeof(PaquetesController), nameof(PaquetesController.Listar))]
    [InlineData(typeof(PaquetesController), nameof(PaquetesController.Buscar))]
    [InlineData(typeof(TicketsController), nameof(TicketsController.ListarGlobales))]
    [InlineData(typeof(TicketsController), nameof(TicketsController.ReportarCliente))]
    [InlineData(typeof(TicketsController), nameof(TicketsController.VerificarCliente))]
    public void EndpointsPublicos_DebenTener_AllowAnonymous(Type controller, string methodName)
    {
        
        var methods = controller
            .GetMethods()
            .Where(m => m.Name == methodName)
            .ToList();

        
        Assert.NotEmpty(methods);
        Assert.Contains(methods, m => m.GetCustomAttribute<AllowAnonymousAttribute>() != null);
    }

    [Theory]
    [InlineData(typeof(TicketsController), nameof(TicketsController.Listar), "Administrador,Tecnico")]
    [InlineData(typeof(TicketsController), nameof(TicketsController.Buscar), "Administrador,Tecnico")]
    [InlineData(typeof(TicketsController), nameof(TicketsController.Crear), "Administrador,Tecnico")]
    [InlineData(typeof(TicketsController), nameof(TicketsController.ActualizarEstado), "Administrador,Tecnico")]
    [InlineData(typeof(InstalacionesController), nameof(InstalacionesController.ListarTecnicos), "Administrador,Vendedor")]
    [InlineData(typeof(InstalacionesController), nameof(InstalacionesController.ProgramarInstalacion), "Administrador,Vendedor")]
    [InlineData(typeof(InstalacionesController), nameof(InstalacionesController.CompletarInstalacion), "Administrador,Tecnico")]
    public void EndpointsProtegidos_DebenTener_AuthorizeConRoles(Type controller, string methodName, string rolesEsperados)
    {
        
        var methods = controller
            .GetMethods()
            .Where(m => m.Name == methodName)
            .ToList();

        
        Assert.NotEmpty(methods);

        var authorize = methods
            .Select(m => m.GetCustomAttribute<AuthorizeAttribute>())
            .FirstOrDefault(a => a != null);

        Assert.NotNull(authorize);
        Assert.Equal(rolesEsperados, authorize!.Roles);
    }
}