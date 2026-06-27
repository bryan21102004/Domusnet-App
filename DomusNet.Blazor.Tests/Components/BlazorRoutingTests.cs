using Microsoft.AspNetCore.Components;
using Xunit;


using DomusNet.Blazor.Pages;

namespace DomusNet.Blazor.Tests.Components;

public class BlazorRoutingTests
{
    [Theory]
    [InlineData(typeof(AccesoInterno), "/acceso-interno")]
    [InlineData(typeof(Paquetes), "/paquetes")]
    [InlineData(typeof(ReportarAveria), "/reportar-averia")]
    [InlineData(typeof(AdminTickets), "/admin/tickets")]
    [InlineData(typeof(TecnicoTickets), "/tecnico/tickets")]
    [InlineData(typeof(SolicitarPaquete), "/solicitar-paquete/{IdPaquete:int}")]
    public void Paginas_DebenTenerRutaCorrecta(Type componente, string rutaEsperada)
    {
        
        var rutas = componente
            .GetCustomAttributes(typeof(RouteAttribute), inherit: true)
            .Cast<RouteAttribute>()
            .Select(r => r.Template)
            .ToList();

        
        Assert.Contains(rutaEsperada, rutas);
    }
}