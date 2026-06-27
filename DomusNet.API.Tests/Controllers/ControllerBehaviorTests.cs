using DomusNet.API.Controllers;
using DomusNet.API.DTOs;
using DomusNet.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DomusNet.API.Tests.Controllers;

public class ControllerBehaviorTests
{
    [Fact]
    public async Task Tickets_Buscar_CuandoNoExiste_DebeRetornarNotFound()
    {
        
        var serviceMock = new Mock<ITicketsService>();

        serviceMock
            .Setup(s => s.BuscarAsync(99))
            .ReturnsAsync((object?)null);

        var controller = new TicketsController(serviceMock.Object);

        
        var resultado = await controller.Buscar(99);

        
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task Tickets_Crear_CuandoServicioRetornaExito_DebeRetornarOk()
    {
        
        var serviceMock = new Mock<ITicketsService>();

        serviceMock
            .Setup(s => s.CrearAsync(It.IsAny<TicketCreateDto>()))
            .ReturnsAsync(new SpResultDto
            {
                Resultado = 1,
                IdGenerado = 10
            });

        var controller = new TicketsController(serviceMock.Object);

        
        var resultado = await controller.Crear(null!);

        
        Assert.IsType<OkObjectResult>(resultado);
    }

    [Fact]
    public async Task Tickets_Crear_CuandoServicioRetornaError_DebeRetornarBadRequest()
    {
    
        var serviceMock = new Mock<ITicketsService>();

        serviceMock
            .Setup(s => s.CrearAsync(It.IsAny<TicketCreateDto>()))
            .ReturnsAsync(new SpResultDto
            {
                Resultado = 0,
                Mensaje = "No se pudo crear"
            });

        var controller = new TicketsController(serviceMock.Object);

        
        var resultado = await controller.Crear(null!);

        
        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task Paquetes_Buscar_CuandoNoExiste_DebeRetornarNotFound()
    {
        
        var serviceMock = new Mock<IPaquetesService>();

        serviceMock
            .Setup(s => s.BuscarAsync(50))
            .ReturnsAsync((DomusNet.API.Data.Models.PaqueteServicio?)null);

        var controller = new PaquetesController(serviceMock.Object);

        
        var resultado = await controller.Buscar(50);

        
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task Paquetes_Crear_CuandoServicioRetornaExito_DebeRetornarOk()
    {
        
        var serviceMock = new Mock<IPaquetesService>();

        serviceMock
            .Setup(s => s.CrearAsync(It.IsAny<PaqueteCreateDto>()))
            .ReturnsAsync(new SpResultDto
            {
                Resultado = 1,
                IdGenerado = 5
            });

        var controller = new PaquetesController(serviceMock.Object);

        
        var resultado = await controller.Crear(null!);

        
        Assert.IsType<OkObjectResult>(resultado);
    }

    [Fact]
    public async Task Paquetes_Editar_CuandoNoExiste_DebeRetornarNotFound()
    {
        
        var serviceMock = new Mock<IPaquetesService>();

        serviceMock
            .Setup(s => s.EditarAsync(99, It.IsAny<PaqueteUpdateDto>()))
            .ReturnsAsync(0);

        var controller = new PaquetesController(serviceMock.Object);

        
        var resultado = await controller.Editar(99, null!);

        
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
public async Task Solicitudes_Crear_CuandoServicioRetornaExito_DebeRetornarOk()
{
    var serviceMock = new Mock<ISolicitudesService>();

    serviceMock
        .Setup(s => s.CrearAsync(It.IsAny<SolicitudCreateDto>()))
        .ReturnsAsync(new SolicitudResultDto
        {
            Resultado = 1,
            IdGenerado = 20,
            Mensaje = "Solicitud registrada correctamente"
        });

    var controller = new SolicitudesController(serviceMock.Object);

    var dto = new SolicitudCreateDto
    {
        NombreCompleto = "Juan Perez",
        Telefono = "88888888",
        Correo = "juan@correo.com",
        Direccion = "Direccion de prueba completa"
    };

    var resultado = await controller.Crear(dto);

    Assert.IsType<OkObjectResult>(resultado);
}

    [Fact]
    public async Task Solicitudes_Atender_CuandoNoExiste_DebeRetornarNotFound()
    {
        
        var serviceMock = new Mock<ISolicitudesService>();

        serviceMock
            .Setup(s => s.AtenderAsync(99, It.IsAny<AtenderSolicitudDto>()))
            .ReturnsAsync(0);

        var controller = new SolicitudesController(serviceMock.Object);

        
        var resultado = await controller.Atender(99, null!);

        
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task Usuarios_Buscar_CuandoNoExiste_DebeRetornarNotFound()
    {
        
        var serviceMock = new Mock<IUsuariosService>();

        serviceMock
            .Setup(s => s.BuscarAsync(99))
            .ReturnsAsync((UsuarioResponseDto?)null);

        var controller = new UsuariosController(serviceMock.Object);

        
        var resultado = await controller.Buscar(99);

        
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task Usuarios_Crear_CuandoCorreoYaExiste_DebeRetornarBadRequest()
    {
        
        var serviceMock = new Mock<IUsuariosService>();

        serviceMock
            .Setup(s => s.CrearAsync(It.IsAny<UsuarioCreateDto>()))
            .ReturnsAsync(new SpResultDto
            {
                Resultado = -1,
                Mensaje = "Correo ya registrado"
            });

        var controller = new UsuariosController(serviceMock.Object);

        
        var resultado = await controller.Crear(null!);

        
        Assert.IsType<BadRequestObjectResult>(resultado);
    }
}

