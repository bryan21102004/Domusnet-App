using System.Text;
using DomusNet.API.Data;
using DomusNet.API.Services;
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

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuariosService>();
builder.Services.AddScoped<PaquetesService>();
builder.Services.AddScoped<ClientesService>();
builder.Services.AddScoped<TicketsService>();
builder.Services.AddScoped<SolicitudesService>();
builder.Services.AddScoped<ValidacionesGenerales>();
builder.Services.AddScoped<InstalacionService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<SolicitudesService>();
builder.Services.AddScoped<IngresosService>();
builder.Services.AddScoped<ReportesService>();
builder.Services.AddScoped<NotificacionesService>();
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
