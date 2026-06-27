using System.Runtime.CompilerServices;
using Bunit;
using Xunit;


using DomusNet.Blazor.Pages;

namespace DomusNet.Blazor.Tests.Components;

public class PublicPagesRenderTests : BunitContext
{
    public PublicPagesRenderTests()
    {
        Services.AddFallbackServiceProvider(new DummyFallbackServiceProvider());
    }

    [Fact]
    public void AccesoInterno_DebeRenderizarFormularioLogin()
    {
        
        var componente = Render<AccesoInterno>();

        
        Assert.Contains("Acceso", componente.Markup);
        Assert.Contains("Interno", componente.Markup);
        Assert.Contains("Correo", componente.Markup);
        Assert.Contains("Contraseña", componente.Markup);
        Assert.Contains("Iniciar Sesión", componente.Markup);

        Assert.Single(componente.FindAll("input[type=email]"));
        Assert.Single(componente.FindAll("input[type=password]"));
        Assert.Single(componente.FindAll("button[type=submit]"));
    }

    [Fact]
    public void AccesoInterno_EnviarVacio_DebeMostrarMensajeValidacion()
    {
        
        var componente = Render<AccesoInterno>();

        
        componente.Find("form").Submit();

       
        componente.WaitForAssertion(() =>
        {
            Assert.Contains("Debe de ingresar su correo y contraseña", componente.Markup);
        });
    }

    [Fact]
    public void ReportarAveria_DebeRenderizarFormularioReporte()
    {
        
        var componente = Render<ReportarAveria>();

        
        Assert.Contains("Reportar", componente.Markup);
        Assert.Contains("Avería", componente.Markup);
        Assert.Contains("Teléfono registrado", componente.Markup);
        Assert.Contains("Nombre completo", componente.Markup);
        Assert.Contains("Dirección", componente.Markup);
        Assert.Contains("Verificar", componente.Markup);

        Assert.NotEmpty(componente.FindAll("input"));
        Assert.NotEmpty(componente.FindAll("button"));
    }

    private sealed class DummyFallbackServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            if (serviceType.IsClass)
            {
                return RuntimeHelpers.GetUninitializedObject(serviceType);
            }

            return null;
        }
    }
}