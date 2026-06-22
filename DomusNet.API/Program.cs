using System.Text;
using DomusNet.API.Data;
using DomusNet.API.Services;
using DomusNet.API.Services.Interfaces;
using DomusNet.API.Services.ReglasNegocio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<DomusNetDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IPaquetesService, PaquetesService>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<ISolicitudesService, SolicitudesService>();
builder.Services.AddScoped< ValidacionesGenerales>();
builder.Services.AddScoped<IInstalacionService, InstalacionService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IIngresosService, IngresosService>();
builder.Services.AddScoped<IReportesService, ReportesService>();
builder.Services.AddScoped<INotificacionesService, NotificacionesService>();
builder.Services.AddScoped<IEvidenciaInstalacionService, EvidenciaInstalacionService>();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DomusNetDBContext>();
    try
    {
        context.Database.OpenConnection();
        Console.WriteLine("Conexion exitosa a SQL Server.");
        context.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error de conexion: " + ex.Message);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
