using System.Runtime.CompilerServices;
using Bunit;
using Xunit;
using System;
using System.Net.Http;

using DomusNet.Blazor.Pages;
using DomusNet.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;


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
    using var ctx = new BunitContext();

    ctx.Services.AddSingleton(new HttpClient
    {
        BaseAddress = new Uri("http://localhost/")
    });

    ctx.Services.AddSingleton<AuthFrontendService>();

    var cut = ctx.Render<AccesoInterno>();

    var boton = cut.Find("button[type='submit']");
    boton.Click();

    cut.WaitForAssertion(() =>
    {
        Assert.Contains("Debe ingresar su correo electrónico.", cut.Markup);
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