using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DomusNet.Blazor;
using DomusNet.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {
     BaseAddress = new Uri("http://localhost:5242")});

builder.Services.AddScoped<AuthFrontendService>();
builder.Services.AddScoped<SolicitudesVendedorService>();
builder.Services.AddScoped<PaquetesFrontendService>();
builder.Services.AddScoped<SolicitudesPublicasService>();
builder.Services.AddScoped<InstalacionesVendedorService>();
builder.Services.AddScoped<TecnicoInstalacionesService>();

await builder.Build().RunAsync();
