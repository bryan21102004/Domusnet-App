
USE DomusNet
GO

DROP PROCEDURE IF EXISTS listarClientes;
GO
CREATE OR ALTER PROCEDURE dbo.listarClientes
    @EstadoPago NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.IdCliente,
        c.NombreCompleto,
        c.Telefono,
        c.Correo,
        c.Direccion,
        c.EstadoPago,
        c.FechaRegistro,
        c.Activo,

        u.Nombre AS NombreVendedor,

        p.Nombre AS NombrePaquete,
        p.Velocidad,
        p.Precio,

        ap.FechaAsignacion,
        ap.Estado AS EstadoAsignacion

    FROM dbo.Clientes c

    INNER JOIN dbo.Usuarios u
        ON c.IdVendedor = u.IdUsuario

    LEFT JOIN dbo.AsignacionesPaquete ap
        ON c.IdCliente = ap.IdCliente
       AND ap.Estado = 'Activa'

    LEFT JOIN dbo.PaquetesServicio p
        ON ap.IdPaquete = p.IdPaquete

    WHERE c.Activo = 1
      AND (@EstadoPago IS NULL OR c.EstadoPago = @EstadoPago)

    ORDER BY c.NombreCompleto;
END;
GO

CREATE OR ALTER PROCEDURE buscarCliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdCliente, c.NombreCompleto, c.Telefono, c.Correo, c.Direccion,
           c.EstadoPago, c.FechaRegistro, c.IdVendedor, u.Nombre AS NombreVendedor,
           c.IdSolicitudOrigen, c.Activo
    FROM Clientes c
    INNER JOIN Usuarios u ON c.IdVendedor = u.IdUsuario
    WHERE c.IdCliente = @IdCliente;
END
GO

-- Retorna: 1=ok, -1=telefono duplicado
CREATE OR ALTER PROCEDURE nuevoCliente
    @NombreCompleto     NVARCHAR(150),
    @Telefono           NVARCHAR(20),
    @Correo             NVARCHAR(150) = NULL,
    @Direccion          NVARCHAR(300),
    @IdVendedor         INT,
    @IdSolicitudOrigen  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Clientes WHERE Telefono = @Telefono AND Activo = 1)
    BEGIN
        SELECT -1 AS Resultado, 0 AS IdGenerado;
        RETURN;
    END

    INSERT INTO Clientes (NombreCompleto, Telefono, Correo, Direccion, EstadoPago,
                          FechaRegistro, IdVendedor, IdSolicitudOrigen, Activo)
    VALUES (@NombreCompleto, @Telefono, @Correo, @Direccion, 'AlDia',
            GETUTCDATE(), @IdVendedor, @IdSolicitudOrigen, 1);

    SELECT 1 AS Resultado, SCOPE_IDENTITY() AS IdGenerado;
END
GO

-- Retorna: 1=ok, 0=no existe, -1=telefono duplicado
CREATE OR ALTER PROCEDURE editarCliente
    @IdCliente          INT,
    @NombreCompleto     NVARCHAR(150),
    @Telefono           NVARCHAR(20),
    @Correo             NVARCHAR(150) = NULL,
    @Direccion          NVARCHAR(300),
    @EstadoPago         NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Clientes WHERE IdCliente = @IdCliente)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Clientes WHERE Telefono = @Telefono AND IdCliente <> @IdCliente AND Activo = 1)
    BEGIN
        SELECT -1 AS Resultado;
        RETURN;
    END

    UPDATE Clientes
    SET NombreCompleto = @NombreCompleto, Telefono = @Telefono, Correo = @Correo,
        Direccion = @Direccion, EstadoPago = @EstadoPago
    WHERE IdCliente = @IdCliente;

    SELECT 1 AS Resultado;
END
GO

-- Retorna: 1=ok, -1=ya asignado activo
CREATE OR ALTER PROCEDURE asignarPaqueteCliente
    @IdCliente      INT,
    @IdPaquete      INT,
    @IdVendedor     INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM AsignacionesPaquete
        WHERE IdCliente = @IdCliente AND IdPaquete = @IdPaquete AND Estado = 'Activa'
    )
    BEGIN
        SELECT -1 AS Resultado, 0 AS IdGenerado;
        RETURN;
    END

    INSERT INTO AsignacionesPaquete (IdCliente, IdPaquete, IdVendedor, FechaAsignacion, Estado)
    VALUES (@IdCliente, @IdPaquete, @IdVendedor, GETUTCDATE(), 'Activa');

    DECLARE @IdAsignacion INT = SCOPE_IDENTITY();
    DECLARE @Precio DECIMAL(10,2);
    DECLARE @Porcentaje DECIMAL(5,2);

    SELECT @Precio = Precio, @Porcentaje = PorcentajeDistribucion
    FROM PaquetesServicio WHERE IdPaquete = @IdPaquete;

    INSERT INTO Ventas (IdAsignacion, FechaVenta, Monto, Estado, PorcentajeDistribucion)
    VALUES (@IdAsignacion, GETUTCDATE(), @Precio, 'Activa', @Porcentaje);

    SELECT 1 AS Resultado, @IdAsignacion AS IdGenerado;
END
GO




-- =============================================
-- FILE: sp_ingresos.sql
-- =============================================

USE DomusNet;
GO

CREATE OR ALTER PROCEDURE listarIngresos
    @Desde  DATETIME2 = NULL,
    @Hasta  DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT i.IdIngreso, i.IdCliente, c.NombreCompleto AS NombreCliente,
           i.IdPaquete, p.Nombre AS NombrePaquete,
           i.Monto, i.Fecha, i.Descripcion, i.IdRegistradoPor, u.Nombre AS NombreRegistradoPor
    FROM Ingresos i
    LEFT JOIN Clientes c ON i.IdCliente = c.IdCliente
    LEFT JOIN PaquetesServicio p ON i.IdPaquete = p.IdPaquete
    INNER JOIN Usuarios u ON i.IdRegistradoPor = u.IdUsuario
    WHERE (@Desde IS NULL OR i.Fecha >= @Desde)
      AND (@Hasta IS NULL OR i.Fecha <= @Hasta)
    ORDER BY i.Fecha DESC;
END
GO

CREATE OR ALTER PROCEDURE buscarIngreso
    @IdIngreso INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT i.IdIngreso, i.IdCliente, c.NombreCompleto AS NombreCliente,
           i.IdPaquete, p.Nombre AS NombrePaquete,
           i.Monto, i.Fecha, i.Descripcion, i.IdRegistradoPor, u.Nombre AS NombreRegistradoPor
    FROM Ingresos i
    LEFT JOIN Clientes c ON i.IdCliente = c.IdCliente
    LEFT JOIN PaquetesServicio p ON i.IdPaquete = p.IdPaquete
    INNER JOIN Usuarios u ON i.IdRegistradoPor = u.IdUsuario
    WHERE i.IdIngreso = @IdIngreso;
END
GO

CREATE OR ALTER PROCEDURE nuevoIngreso
    @IdCliente        INT = NULL,
    @IdPaquete        INT = NULL,
    @Monto            DECIMAL(10,2),
    @Descripcion      NVARCHAR(300) = NULL,
    @IdRegistradoPor  INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Monto <= 0
    BEGIN
        SELECT -1 AS Resultado, 0 AS IdGenerado;
        RETURN;
    END

    INSERT INTO Ingresos (IdCliente, IdPaquete, Monto, Fecha, Descripcion, IdRegistradoPor)
    VALUES (@IdCliente, @IdPaquete, @Monto, GETUTCDATE(), @Descripcion, @IdRegistradoPor);

    SELECT 1 AS Resultado, SCOPE_IDENTITY() AS IdGenerado;
END
GO

CREATE OR ALTER PROCEDURE resumenIngresos
    @Mes  INT = NULL,
    @Anio INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ISNULL(SUM(Monto), 0) AS TotalIngresos,
        COUNT(*) AS CantidadRegistros
    FROM Ingresos
    WHERE (@Mes IS NULL OR MONTH(Fecha) = @Mes)
      AND (@Anio IS NULL OR YEAR(Fecha) = @Anio);
END
GO




-- =============================================
-- FILE: sp_notificaciones.sql
-- =============================================

USE DomusNet;
GO

CREATE OR ALTER PROCEDURE listarNotificaciones
    @IdUsuarioDestino INT,
    @SoloNoLeidas    BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT n.IdNotificacion, n.IdUsuarioDestino, n.IdTicket, n.Mensaje,
           n.Leida, n.FechaEnvio, t.Titulo AS TituloTicket
    FROM Notificaciones n
    LEFT JOIN Tickets t ON n.IdTicket = t.IdTicket
    WHERE n.IdUsuarioDestino = @IdUsuarioDestino
      AND (@SoloNoLeidas = 0 OR n.Leida = 0)
    ORDER BY n.FechaEnvio DESC;
END
GO

CREATE OR ALTER PROCEDURE contarNotificacionesNoLeidas
    @IdUsuarioDestino INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS TotalNoLeidas
    FROM Notificaciones
    WHERE IdUsuarioDestino = @IdUsuarioDestino AND Leida = 0;
END
GO

-- Retorna: 1=ok, 0=no existe o no pertenece al usuario
CREATE OR ALTER PROCEDURE marcarNotificacionLeida
    @IdNotificacion   INT,
    @IdUsuarioDestino INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM Notificaciones
        WHERE IdNotificacion = @IdNotificacion AND IdUsuarioDestino = @IdUsuarioDestino
    )
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE Notificaciones SET Leida = 1 WHERE IdNotificacion = @IdNotificacion;
    SELECT 1 AS Resultado;
END
GO

-- Retorna: cantidad marcadas
CREATE OR ALTER PROCEDURE marcarTodasNotificacionesLeidas
    @IdUsuarioDestino INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Notificaciones SET Leida = 1
    WHERE IdUsuarioDestino = @IdUsuarioDestino AND Leida = 0;
    SELECT @@ROWCOUNT AS Resultado;
END
GO




-- =============================================
-- FILE: sp_paquetes.sql
-- =============================================

UPDATE dbo.SolicitudesServicio
SET 
    Estado = 'Realizada',
    Notas = 'Instalación realizada por el técnico. Lista para convertir a cliente.'
WHERE IdSolicitud = 14;
GO


SELECT 
    s.IdSolicitud,
    s.Estado AS EstadoSolicitud,
    i.IdInstalacion,
    i.Estado AS EstadoInstalacion
FROM dbo.SolicitudesServicio s
INNER JOIN dbo.InstalacionesProgramadas i
    ON s.IdSolicitud = i.IdSolicitud
WHERE s.IdSolicitud = 14;

USE DomusNet;
GO

CREATE OR ALTER PROCEDURE listarPaquetes
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPaquete, Nombre, Descripcion, Velocidad, Precio,
           PorcentajeDistribucion, Estado, FechaCreacion
    FROM PaquetesServicio
    WHERE @SoloActivos = 0 OR Estado = 'Activo'
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE buscarPaquete
    @IdPaquete INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPaquete, Nombre, Descripcion, Velocidad, Precio,
           PorcentajeDistribucion, Estado, FechaCreacion
    FROM PaquetesServicio
    WHERE IdPaquete = @IdPaquete;
END
GO

CREATE OR ALTER PROCEDURE nuevoPaquete
    @Nombre                 NVARCHAR(100),
    @Descripcion            NVARCHAR(500) = NULL,
    @Velocidad              NVARCHAR(50) = NULL,
    @Precio                 DECIMAL(10,2),
    @PorcentajeDistribucion DECIMAL(5,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PaquetesServicio (Nombre, Descripcion, Velocidad, Precio, PorcentajeDistribucion, Estado, FechaCreacion)
    VALUES (@Nombre, @Descripcion, @Velocidad, @Precio, @PorcentajeDistribucion, 'Activo', GETUTCDATE());

    SELECT 1 AS Resultado, SCOPE_IDENTITY() AS IdGenerado;
END
GO

-- Retorna: 1=ok, 0=no existe
CREATE OR ALTER PROCEDURE editarPaquete
    @IdPaquete              INT,
    @Nombre                 NVARCHAR(100),
    @Descripcion            NVARCHAR(500) = NULL,
    @Velocidad              NVARCHAR(50) = NULL,
    @Precio                 DECIMAL(10,2),
    @PorcentajeDistribucion DECIMAL(5,2) = NULL,
    @Estado                 NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM PaquetesServicio WHERE IdPaquete = @IdPaquete)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE PaquetesServicio
    SET Nombre = @Nombre, Descripcion = @Descripcion, Velocidad = @Velocidad,
        Precio = @Precio, PorcentajeDistribucion = @PorcentajeDistribucion, Estado = @Estado
    WHERE IdPaquete = @IdPaquete;

    SELECT 1 AS Resultado;
END
GO




-- =============================================
-- FILE: sp_reportes.sql
-- =============================================

USE DomusNet;
GO

CREATE OR ALTER PROCEDURE obtenerDashboard
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM Clientes WHERE Activo = 1) AS TotalClientes,
        (SELECT COUNT(*) FROM Tickets) AS TotalTickets,
        (SELECT COUNT(*) FROM Tickets WHERE Estado IN ('Pendiente', 'EnProceso')) AS TicketsAbiertos,
        (SELECT COUNT(*) FROM Tickets WHERE Estado = 'Resuelto') AS TicketsResueltos,
        (SELECT ISNULL(SUM(Monto), 0) FROM Ingresos) AS TotalIngresos,
        (SELECT ISNULL(SUM(Monto), 0) FROM Ingresos
         WHERE MONTH(Fecha) = MONTH(GETUTCDATE()) AND YEAR(Fecha) = YEAR(GETUTCDATE())) AS IngresosMesActual,
        (SELECT COUNT(*) FROM SolicitudesServicio WHERE Estado = 'Pendiente') AS SolicitudesPendientes,
        (SELECT COUNT(*) FROM Clientes WHERE EstadoPago = 'Moroso' AND Activo = 1) AS ClientesMorosos;
END
GO

CREATE OR ALTER PROCEDURE reporteIngresos
    @Desde DATETIME2 = NULL,
    @Hasta DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT i.IdIngreso, i.Monto, i.Fecha, i.Descripcion,
           c.NombreCompleto AS Cliente, p.Nombre AS Paquete, u.Nombre AS RegistradoPor
    FROM Ingresos i
    LEFT JOIN Clientes c ON i.IdCliente = c.IdCliente
    LEFT JOIN PaquetesServicio p ON i.IdPaquete = p.IdPaquete
    INNER JOIN Usuarios u ON i.IdRegistradoPor = u.IdUsuario
    WHERE (@Desde IS NULL OR i.Fecha >= @Desde)
      AND (@Hasta IS NULL OR i.Fecha <= @Hasta)
    ORDER BY i.Fecha DESC;

    SELECT ISNULL(SUM(Monto), 0) AS TotalPeriodo, COUNT(*) AS Cantidad
    FROM Ingresos
    WHERE (@Desde IS NULL OR Fecha >= @Desde)
      AND (@Hasta IS NULL OR Fecha <= @Hasta);
END
GO

CREATE OR ALTER PROCEDURE reporteClientes
    @EstadoPago NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.IdCliente, c.NombreCompleto, c.Telefono, c.Correo,
           c.EstadoPago, c.FechaRegistro, u.Nombre AS Vendedor, c.Activo
    FROM Clientes c
    INNER JOIN Usuarios u ON c.IdVendedor = u.IdUsuario
    WHERE (@EstadoPago IS NULL OR c.EstadoPago = @EstadoPago)
    ORDER BY c.NombreCompleto;

    SELECT
        COUNT(*) AS TotalClientes,
        SUM(CASE WHEN EstadoPago = 'AlDia' THEN 1 ELSE 0 END) AS AlDia,
        SUM(CASE WHEN EstadoPago = 'Moroso' THEN 1 ELSE 0 END) AS Morosos,
        SUM(CASE WHEN EstadoPago = 'Pendiente' THEN 1 ELSE 0 END) AS Pendientes
    FROM Clientes
    WHERE Activo = 1
      AND (@EstadoPago IS NULL OR EstadoPago = @EstadoPago);
END
GO

CREATE OR ALTER PROCEDURE reporteTickets
    @Estado NVARCHAR(20) = NULL,
    @Tipo   NVARCHAR(20) = NULL,
    @Desde  DATETIME2 = NULL,
    @Hasta  DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.IdTicket, t.Titulo, t.Tipo, t.Estado, t.Prioridad,
           t.FechaCreacion, t.FechaCierre,
           c.NombreCompleto AS Cliente, uc.Nombre AS CreadoPor, ua.Nombre AS AsignadoA
    FROM Tickets t
    LEFT JOIN Clientes c ON t.IdCliente = c.IdCliente
    INNER JOIN Usuarios uc ON t.IdCreadoPor = uc.IdUsuario
    LEFT JOIN Usuarios ua ON t.IdAsignadoA = ua.IdUsuario
    WHERE (@Estado IS NULL OR t.Estado = @Estado)
      AND (@Tipo IS NULL OR t.Tipo = @Tipo)
      AND (@Desde IS NULL OR t.FechaCreacion >= @Desde)
      AND (@Hasta IS NULL OR t.FechaCreacion <= @Hasta)
    ORDER BY t.FechaCreacion DESC;

    SELECT
        COUNT(*) AS Total,
        SUM(CASE WHEN Estado = 'Pendiente' THEN 1 ELSE 0 END) AS Pendientes,
        SUM(CASE WHEN Estado = 'EnProceso' THEN 1 ELSE 0 END) AS EnProceso,
        SUM(CASE WHEN Estado = 'Resuelto' THEN 1 ELSE 0 END) AS Resueltos
    FROM Tickets
    WHERE (@Estado IS NULL OR Estado = @Estado)
      AND (@Tipo IS NULL OR Tipo = @Tipo)
      AND (@Desde IS NULL OR FechaCreacion >= @Desde)
      AND (@Hasta IS NULL OR FechaCreacion <= @Hasta);
END
GO

CREATE OR ALTER PROCEDURE guardarReporte
    @Tipo          NVARCHAR(30),
    @Parametros    NVARCHAR(1000) = NULL,
    @IdGeneradoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Reportes (Tipo, Parametros, FechaGeneracion, IdGeneradoPor)
    VALUES (@Tipo, @Parametros, GETUTCDATE(), @IdGeneradoPor);
    SELECT 1 AS Resultado, SCOPE_IDENTITY() AS IdGenerado;
END
GO

CREATE OR ALTER PROCEDURE listarReportesGenerados
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.IdReporte, r.Tipo, r.Parametros, r.FechaGeneracion,
           u.Nombre AS GeneradoPor
    FROM Reportes r
    INNER JOIN Usuarios u ON r.IdGeneradoPor = u.IdUsuario
    ORDER BY r.FechaGeneracion DESC;
END
GO

SELECT IdUsuario, Nombre, RefreshToken FROM Usuarios WHERE IdUsuario = 1;


SELECT * FROM PaquetesServicio
-- =============================================
-- FILE: sp_solicitudes.sql
-- =============================================

USE DomusNet;
GO

-- Retorna: 1=ok, IdGenerado = IdSolicitud
CREATE OR ALTER PROCEDURE nuevaSolicitud
    @NombreCompleto NVARCHAR(150),
    @Telefono       NVARCHAR(20),
    @Correo         NVARCHAR(150) = NULL,
    @Direccion      NVARCHAR(300),
    @IdPaquete      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ClientesExternos (NombreCompleto, Telefono, Correo, Direccion)
    VALUES (@NombreCompleto, @Telefono, @Correo, @Direccion);

    DECLARE @IdClienteExterno INT = SCOPE_IDENTITY();

    INSERT INTO SolicitudesServicio (IdClienteExterno, IdPaquete, FechaSolicitud, Estado)
    VALUES (@IdClienteExterno, @IdPaquete, GETUTCDATE(), 'Pendiente');

    SELECT 1 AS Resultado, SCOPE_IDENTITY() AS IdGenerado, @IdClienteExterno AS IdClienteExterno;
END
GO

CREATE OR ALTER PROCEDURE listarSolicitudes
    @Estado NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.IdSolicitud, s.FechaSolicitud, s.Estado, s.Notas,
           s.IdVendedorAsignado, uv.Nombre AS NombreVendedor,
           ce.NombreCompleto, ce.Telefono, ce.Correo, ce.Direccion,
           p.IdPaquete, p.Nombre AS NombrePaquete, p.Precio
    FROM SolicitudesServicio s
    INNER JOIN ClientesExternos ce ON s.IdClienteExterno = ce.IdClienteExterno
    LEFT JOIN PaquetesServicio p ON s.IdPaquete = p.IdPaquete
    LEFT JOIN Usuarios uv ON s.IdVendedorAsignado = uv.IdUsuario
    WHERE @Estado IS NULL OR s.Estado = @Estado
    ORDER BY s.FechaSolicitud DESC;
END
GO

-- Retorna: 1=ok, 0=no existe
CREATE OR ALTER PROCEDURE atenderSolicitud
    @IdSolicitud        INT,
    @IdVendedorAsignado INT,
    @Notas              NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SolicitudesServicio WHERE IdSolicitud = @IdSolicitud)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE SolicitudesServicio
    SET Estado = 'Atendida', IdVendedorAsignado = @IdVendedorAsignado, Notas = @Notas
    WHERE IdSolicitud = @IdSolicitud;

    SELECT 1 AS Resultado;
END
GO



-- =============================================
-- FILE: sp_tickets.sql
-- =============================================

USE DomusNet;
GO

CREATE OR ALTER PROCEDURE listarTickets
    @Estado         NVARCHAR(20) = NULL,
    @IdAsignadoA    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.IdTicket, t.Titulo, t.Descripcion, t.Tipo, t.Estado, t.Prioridad,
           t.IdCliente, c.NombreCompleto AS NombreCliente,
           t.IdCreadoPor, uc.Nombre AS NombreCreador,
           t.IdAsignadoA, ua.Nombre AS NombreAsignado,
           t.FechaCreacion, t.FechaActualizacion, t.FechaCierre, t.EsGlobal
    FROM Tickets t
    LEFT JOIN Clientes c ON t.IdCliente = c.IdCliente
    INNER JOIN Usuarios uc ON t.IdCreadoPor = uc.IdUsuario
    LEFT JOIN Usuarios ua ON t.IdAsignadoA = ua.IdUsuario
    WHERE (@Estado IS NULL OR t.Estado = @Estado)
      AND (@IdAsignadoA IS NULL OR t.IdAsignadoA = @IdAsignadoA)
    ORDER BY t.FechaCreacion DESC;
END
GO

CREATE OR ALTER PROCEDURE buscarTicket
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.IdTicket, t.Titulo, t.Descripcion, t.Tipo, t.Estado, t.Prioridad,
           t.IdCliente, c.NombreCompleto AS NombreCliente,
           t.IdCreadoPor, uc.Nombre AS NombreCreador,
           t.IdAsignadoA, ua.Nombre AS NombreAsignado,
           t.FechaCreacion, t.FechaActualizacion, t.FechaCierre, t.EsGlobal
    FROM Tickets t
    LEFT JOIN Clientes c ON t.IdCliente = c.IdCliente
    INNER JOIN Usuarios uc ON t.IdCreadoPor = uc.IdUsuario
    LEFT JOIN Usuarios ua ON t.IdAsignadoA = ua.IdUsuario
    WHERE t.IdTicket = @IdTicket;
END
GO

CREATE OR ALTER PROCEDURE listarHistorialTicket
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.IdHistorial, h.EstadoAnterior, h.EstadoNuevo, h.Comentario,
           h.IdUsuario, u.Nombre AS NombreUsuario, h.Fecha
    FROM HistorialTickets h
    INNER JOIN Usuarios u ON h.IdUsuario = u.IdUsuario
    WHERE h.IdTicket = @IdTicket
    ORDER BY h.Fecha DESC;
END
GO

CREATE OR ALTER PROCEDURE nuevoTicket
    @Titulo         NVARCHAR(200),
    @Descripcion    NVARCHAR(1000) = NULL,
    @Tipo           NVARCHAR(20),
    @Prioridad      NVARCHAR(10) = 'Media',
    @IdCliente      INT = NULL,
    @IdCreadoPor    INT,
    @IdAsignadoA    INT = NULL,
    @EsGlobal       BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Tickets (Titulo, Descripcion, Tipo, Estado, Prioridad, IdCliente,
                         IdCreadoPor, IdAsignadoA, FechaCreacion, EsGlobal)
    VALUES (@Titulo, @Descripcion, @Tipo, 'Pendiente', @Prioridad, @IdCliente,
            @IdCreadoPor, @IdAsignadoA, GETUTCDATE(), @EsGlobal);

    DECLARE @IdTicket INT = SCOPE_IDENTITY();

    INSERT INTO HistorialTickets (IdTicket, EstadoAnterior, EstadoNuevo, Comentario, IdUsuario, Fecha)
    VALUES (@IdTicket, NULL, 'Pendiente', 'Ticket creado', @IdCreadoPor, GETUTCDATE());

    IF @IdAsignadoA IS NOT NULL
    BEGIN
        INSERT INTO Notificaciones (IdUsuarioDestino, IdTicket, Mensaje, Leida, FechaEnvio)
        VALUES (@IdAsignadoA, @IdTicket, 'Se le ha asignado un nuevo ticket: ' + @Titulo, 0, GETUTCDATE());
    END
    ELSE
    BEGIN
        INSERT INTO Notificaciones (IdUsuarioDestino, IdTicket, Mensaje, Leida, FechaEnvio)
        SELECT u.IdUsuario, @IdTicket,
               CASE WHEN @EsGlobal = 1
                    THEN 'Nueva avería global reportada: ' + @Titulo
                    ELSE 'Nuevo reporte de avería: ' + @Titulo
               END,
               0, GETUTCDATE()
        FROM Usuarios u
        INNER JOIN Roles r ON u.IdRol = r.IdRol
        WHERE r.NombreRol IN ('Administrador', 'Tecnico') AND u.Activo = 1;
    END

    SELECT 1 AS Resultado, @IdTicket AS IdGenerado;
END
GO


CREATE OR ALTER PROCEDURE actualizarEstadoTicket
    @IdTicket       INT,
    @EstadoNuevo    NVARCHAR(20),
    @IdUsuario      INT,
    @Comentario     NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoAnterior NVARCHAR(20);

    SELECT @EstadoAnterior = Estado FROM Tickets WHERE IdTicket = @IdTicket;

    IF @EstadoAnterior IS NULL
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE Tickets
    SET Estado = @EstadoNuevo,
        FechaActualizacion = GETUTCDATE(),
        FechaCierre = CASE WHEN @EstadoNuevo = 'Resuelto' THEN GETUTCDATE() ELSE FechaCierre END
    WHERE IdTicket = @IdTicket;

    INSERT INTO HistorialTickets (IdTicket, EstadoAnterior, EstadoNuevo, Comentario, IdUsuario, Fecha)
    VALUES (@IdTicket, @EstadoAnterior, @EstadoNuevo, @Comentario, @IdUsuario, GETUTCDATE());

    IF @EstadoNuevo = 'Resuelto'
    BEGIN
        INSERT INTO Notificaciones (IdUsuarioDestino, IdTicket, Mensaje, Leida, FechaEnvio)
        SELECT IdCreadoPor, @IdTicket, 'El ticket ha sido resuelto.', 0, GETUTCDATE()
        FROM Tickets WHERE IdTicket = @IdTicket;
    END

    SELECT 1 AS Resultado;
END
GO

CREATE OR ALTER PROCEDURE listarTicketsGlobales
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdTicket, Titulo, Descripcion, Tipo, Estado, Prioridad, FechaCreacion
    FROM Tickets
    WHERE EsGlobal = 1 AND Estado <> 'Resuelto'
    ORDER BY FechaCreacion DESC;
END
GO

CREATE OR ALTER PROCEDURE buscarClienteActivoPorTelefono
    @Telefono NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 IdCliente, NombreCompleto, Direccion
    FROM Clientes
    WHERE Telefono = @Telefono AND Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE listarClientesActivosConCorreo
AS
BEGIN
    SET NOCOUNT ON;
    SELECT NombreCompleto, Correo
    FROM Clientes
    WHERE Activo = 1
      AND Correo IS NOT NULL
      AND LTRIM(RTRIM(Correo)) <> '';
END
GO




-- =============================================
-- FILE: sp_usuarios.sql
-- =============================================

USE DomusNet;
GO

CREATE OR ALTER PROCEDURE listarUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Correo, u.Telefono, u.IdRol,
           r.NombreRol, u.Activo, u.FechaCreacion
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    ORDER BY u.Nombre;
END
GO

CREATE OR ALTER PROCEDURE buscarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Correo, u.Telefono, u.IdRol,
           r.NombreRol, u.Activo, u.FechaCreacion
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.IdUsuario = @IdUsuario;
END
GO

-- Retorna: 1=ok, -1=correo duplicado, -2=rol invalido
CREATE OR ALTER PROCEDURE nuevoUsuario
    @Nombre         NVARCHAR(100),
    @Correo         NVARCHAR(150),
    @Contrasena     NVARCHAR(255),
    @Telefono       NVARCHAR(20) = NULL,
    @IdRol          INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Roles WHERE IdRol = @IdRol)
    BEGIN
        SELECT -2 AS Resultado, 0 AS IdGenerado;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND Activo = 1)
    BEGIN
        SELECT -1 AS Resultado, 0 AS IdGenerado;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND Activo = 0)
    BEGIN
        UPDATE Usuarios
        SET Nombre = @Nombre,
            Contrasena = @Contrasena,
            Telefono = @Telefono,
            IdRol = @IdRol,
            Activo = 1,
            FechaModificacion = GETUTCDATE()
        WHERE Correo = @Correo;

        SELECT 1 AS Resultado, (SELECT IdUsuario FROM Usuarios WHERE Correo = @Correo) AS IdGenerado;
        RETURN;
    END

    INSERT INTO Usuarios (Nombre, Correo, Contrasena, Telefono, IdRol, Activo, FechaCreacion)
    VALUES (@Nombre, @Correo, @Contrasena, @Telefono, @IdRol, 1, GETUTCDATE());

    SELECT 1 AS Resultado, SCOPE_IDENTITY() AS IdGenerado;
END
GO

-- Retorna: 1=ok, 0=no existe, -1=correo duplicado, -2=rol invalido
CREATE OR ALTER PROCEDURE editarUsuario
    @IdUsuario      INT,
    @Nombre         NVARCHAR(100),
    @Correo         NVARCHAR(150),
    @Telefono       NVARCHAR(20) = NULL,
    @IdRol          INT,
    @Activo         BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Roles WHERE IdRol = @IdRol)
    BEGIN
        SELECT -2 AS Resultado;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND IdUsuario <> @IdUsuario AND Activo = 1)
    BEGIN
        SELECT -1 AS Resultado;
        RETURN;
    END

    UPDATE Usuarios
    SET Nombre = @Nombre, Correo = @Correo, Telefono = @Telefono,
        IdRol = @IdRol, Activo = @Activo, FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;

    SELECT 1 AS Resultado;
END
GO

-- Retorna: 1=ok, 0=no existe
CREATE OR ALTER PROCEDURE eliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE Usuarios SET Activo = 0, FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;

    SELECT 1 AS Resultado;
END
GO

-- Retorna: 1=ok, 0=no existe
CREATE OR ALTER PROCEDURE cambiarContrasenaUsuario
    @IdUsuario      INT,
    @Contrasena     NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE Usuarios
    SET Contrasena = @Contrasena, FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;

    SELECT 1 AS Resultado;
END
GO




-- =============================================
-- FILE: seed_admin.sql
-- =============================================

USE DomusNet;
GO

-- Usuario admin inicial
-- Correo: admin@domusnet.com
-- Password: Admin123!

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'admin@domusnet.com')
BEGIN
    INSERT INTO Usuarios (Nombre, Correo, Contrasena, Telefono, IdRol, Activo, FechaCreacion)
    VALUES (
        'Administrador',
        'admin@domusnet.com',
        'AQAAAAIAAYagAAAAEBP+Llw+kKxYArSpKHE5gTBgsYfbraKgLQwbdyxt5IvBmoNxtjPgeyFjs1mDMxZtTA==',
        '88880000',
        1,
        1,
        GETUTCDATE()
    );
    PRINT 'Admin creado: admin@domusnet.com / Admin123!';
END
ELSE
    PRINT 'El admin ya existe.';
GO

-- Paquetes
IF NOT EXISTS (SELECT 1 FROM PaquetesServicio)
BEGIN
    INSERT INTO PaquetesServicio (Nombre, Descripcion, Velocidad, Precio, PorcentajeDistribucion, Estado, FechaCreacion)
    VALUES
        ('Plan Económico',  'Internet Económico',  '10/8 Mbss',  22000, 10, 'Activo', GETUTCDATE()),
        ('Plan Básico','Internet Básico', '15/10 Mbss',  25000, 10, 'Activo', GETUTCDATE()),
        ('Plan Regular ','Internet Regular',  '20/25 Mbss', 29900, 10, 'Activo', GETUTCDATE()),
		('Plan Profesional','Internet Profesional',  '30/20 Mbss',41000, 10, 'Activo', GETUTCDATE())
END
GO
CREATE OR ALTER PROCEDURE listarTecnicosActivos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.IdUsuario,
        u.Nombre,
        u.Correo,
        u.Telefono
    FROM Usuarios u
    INNER JOIN Roles r 
        ON u.IdRol = r.IdRol
    WHERE r.NombreRol = 'Tecnico'
      AND u.Activo = 1
    ORDER BY u.Nombre;
END
GO




DROP PROCEDURE IF EXISTS dbo.programarInstalacion;
GO
CREATE OR ALTER PROCEDURE dbo.programarInstalacion
    @IdSolicitud INT,
    @IdTecnicoAsignado INT,
    @IdVendedorPrograma INT,
    @FechaProgramada DATETIME2,
    @UbicacionInstalacion NVARCHAR(500) = NULL,
    @NotasVendedor NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @IdInstalacion INT;

        -- Validar que la solicitud exista y esté atendida
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.SolicitudesServicio
            WHERE IdSolicitud = @IdSolicitud
              AND Estado = 'Atendida'
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                0 AS Resultado,
                NULL AS IdInstalacion,
                'La solicitud no existe o no está atendida.' AS Mensaje;
            RETURN;
        END

        -- Evitar programar dos veces la misma solicitud
        IF EXISTS (
            SELECT 1
            FROM dbo.InstalacionesProgramadas
            WHERE IdSolicitud = @IdSolicitud
              AND Estado IN ('Programada', 'Completada')
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -3 AS Resultado,
                NULL AS IdInstalacion,
                'La solicitud ya tiene una instalación programada o completada.' AS Mensaje;
            RETURN;
        END

        -- Validar que el técnico exista y sea técnico activo
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Usuarios u
            INNER JOIN dbo.Roles r 
                ON u.IdRol = r.IdRol
            WHERE u.IdUsuario = @IdTecnicoAsignado
              AND r.NombreRol = 'Tecnico'
              AND u.Activo = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -1 AS Resultado,
                NULL AS IdInstalacion,
                'El usuario seleccionado no es un técnico activo.' AS Mensaje;
            RETURN;
        END

-- Validar que quien programa sea vendedor o administrador activo
IF NOT EXISTS (
    SELECT 1
    FROM dbo.Usuarios u
    INNER JOIN dbo.Roles r 
        ON u.IdRol = r.IdRol
    WHERE u.IdUsuario = @IdVendedorPrograma
      AND r.NombreRol IN ('Vendedor', 'Administrador')
      AND u.Activo = 1
)
BEGIN
    ROLLBACK TRANSACTION;

    SELECT 
        -2 AS Resultado,
        NULL AS IdInstalacion,
        'El usuario seleccionado no tiene permisos para programar instalaciones.' AS Mensaje;
    RETURN;
END

        -- Crear instalación programada
        INSERT INTO dbo.InstalacionesProgramadas (
            IdSolicitud,
            IdTecnicoAsignado,
            IdVendedorPrograma,
            FechaProgramada,
            UbicacionInstalacion,
            Estado,
            NotasVendedor
        )
        VALUES (
            @IdSolicitud,
            @IdTecnicoAsignado,
            @IdVendedorPrograma,
            @FechaProgramada,
            @UbicacionInstalacion,
            'Programada',
            @NotasVendedor
        );

        SET @IdInstalacion = SCOPE_IDENTITY();

        -- Cambiar la solicitud a Programada
        UPDATE dbo.SolicitudesServicio
        SET 
            Estado = 'Programada',
            IdVendedorAsignado = @IdVendedorPrograma,
            Notas = COALESCE(@NotasVendedor, Notas)
        WHERE IdSolicitud = @IdSolicitud;

        COMMIT TRANSACTION;

        SELECT 
            1 AS Resultado,
            @IdInstalacion AS IdInstalacion,
            'Instalación programada correctamente.' AS Mensaje;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            -99 AS Resultado,
            NULL AS IdInstalacion,
            ERROR_MESSAGE() AS Mensaje;
    END CATCH
END;
GO


-- ────────────────────────────────────────────────────
--  LISTAR INSTALACIONES POR TÉCNICO
-- ────────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE listarInstalacionesPorTecnico
    @IdTecnico INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.IdInstalacion,
        i.IdSolicitud,

        i.IdTecnicoAsignado,
        i.IdVendedorPrograma,

        i.FechaProgramada,
        i.FechaRealizacion,

        i.UbicacionInstalacion,
        i.FotoEvidencia,
        i.PruebaVelocidad,

        i.Estado,
        i.NotasVendedor,
        i.ComentarioTecnico,
        i.FechaCreacion,
        i.FechaActualizacion,

        ce.NombreCompleto AS NombreCliente,
        ce.NombreCompleto,
        ce.Telefono,
        ce.Correo,
        ce.Direccion,

        p.Nombre AS NombrePaquete

    FROM dbo.InstalacionesProgramadas i
    INNER JOIN dbo.SolicitudesServicio s
        ON i.IdSolicitud = s.IdSolicitud

    INNER JOIN dbo.ClientesExternos ce
        ON s.IdClienteExterno = ce.IdClienteExterno

    LEFT JOIN dbo.PaquetesServicio p
        ON s.IdPaquete = p.IdPaquete

    WHERE i.IdTecnicoAsignado = @IdTecnico
      AND i.Estado IN ('Programada', 'Reprogramada', 'Realizada')

    ORDER BY 
        i.FechaProgramada ASC,
        i.IdInstalacion ASC;
END;
GO

DROP PROCEDURE IF EXISTS completarInstalacion;
GO


CREATE OR ALTER PROCEDURE dbo.completarInstalacion
    @IdInstalacion      INT,
    @FotoEvidencia      NVARCHAR(MAX),
    @PruebaVelocidad    NVARCHAR(500),
    @ComentarioTecnico  NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @IdSolicitud INT;

        SELECT 
            @IdSolicitud = IdSolicitud
        FROM dbo.InstalacionesProgramadas
        WHERE IdInstalacion = @IdInstalacion
          AND Estado IN ('Programada', 'Reprogramada');

        IF @IdSolicitud IS NULL
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -1 AS Resultado, 
                'La instalación no existe o no está en estado válido para realizarse.' AS Mensaje, 
                @IdInstalacion AS IdInstalacion;

            RETURN;
        END

        IF @FotoEvidencia IS NULL OR LTRIM(RTRIM(@FotoEvidencia)) = ''
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -2 AS Resultado, 
                'Debe ingresar la foto de evidencia.' AS Mensaje, 
                @IdInstalacion AS IdInstalacion;

            RETURN;
        END

        IF @PruebaVelocidad IS NULL OR LTRIM(RTRIM(@PruebaVelocidad)) = ''
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -3 AS Resultado, 
                'Debe ingresar la prueba de velocidad.' AS Mensaje, 
                @IdInstalacion AS IdInstalacion;

            RETURN;
        END

        UPDATE dbo.InstalacionesProgramadas
        SET 
            Estado = 'Realizada',
            FechaRealizacion = GETDATE(),
            FotoEvidencia = @FotoEvidencia,
            PruebaVelocidad = @PruebaVelocidad,
            ComentarioTecnico = @ComentarioTecnico,
            FechaActualizacion = GETDATE()
        WHERE IdInstalacion = @IdInstalacion;

        UPDATE dbo.SolicitudesServicio
        SET
            Estado = 'Realizada',
            Notas = 'Instalación realizada por el técnico. Lista para convertir a cliente.'
        WHERE IdSolicitud = @IdSolicitud;

        COMMIT TRANSACTION;

        SELECT 
            1 AS Resultado, 
            'Instalación realizada correctamente. Ya puede ser convertida a cliente por el vendedor.' AS Mensaje, 
            @IdInstalacion AS IdInstalacion;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            -99 AS Resultado, 
            ERROR_MESSAGE() AS Mensaje, 
            @IdInstalacion AS IdInstalacion;
    END CATCH
END;
GO

DROP PROCEDURE IF EXISTS dbo.convertirSolicitudEnCliente;
GO
CREATE OR ALTER PROCEDURE dbo.convertirSolicitudEnCliente
    @IdSolicitud INT,
    @IdVendedor INT,
    @Notas NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE 
            @IdClienteExterno INT,
            @IdPaquete INT,
            @Estado NVARCHAR(30),
            @NombreCompleto NVARCHAR(150),
            @Telefono NVARCHAR(20),
            @Correo NVARCHAR(150),
            @Direccion NVARCHAR(300),
            @IdCliente INT,
            @IdAsignacion INT,
            @Precio DECIMAL(10,2),
            @Porcentaje DECIMAL(5,2),
            @IdInstalacion INT,
            @EstadoInstalacion NVARCHAR(30),
            @FotoEvidencia NVARCHAR(500),
            @PruebaVelocidad NVARCHAR(300),
            @FechaRealizacion DATETIME2;

        SELECT 
            @IdClienteExterno = s.IdClienteExterno,
            @IdPaquete = s.IdPaquete,
            @Estado = s.Estado,
            @NombreCompleto = ce.NombreCompleto,
            @Telefono = ce.Telefono,
            @Correo = ce.Correo,
            @Direccion = ce.Direccion
        FROM dbo.SolicitudesServicio s
        INNER JOIN dbo.ClientesExternos ce
            ON s.IdClienteExterno = ce.IdClienteExterno
        WHERE s.IdSolicitud = @IdSolicitud;

        IF @IdClienteExterno IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 0 AS Resultado, 0 AS IdGenerado, 'La solicitud no existe.' AS Mensaje;
            RETURN;
        END

        IF @Estado = 'Cancelada'
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1 AS Resultado, 0 AS IdGenerado, 'La solicitud está cancelada.' AS Mensaje;
            RETURN;
        END

        IF @Estado = 'Convertida'
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -2 AS Resultado, 0 AS IdGenerado, 'La solicitud ya fue convertida en cliente.' AS Mensaje;
            RETURN;
        END

        IF @Estado <> 'Realizada'
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -12 AS Resultado, 0 AS IdGenerado, 'La solicitud todavía no está realizada. Primero el técnico debe completar la instalación.' AS Mensaje;
            RETURN;
        END

        IF @IdPaquete IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -3 AS Resultado, 0 AS IdGenerado, 'La solicitud no tiene paquete asignado.' AS Mensaje;
            RETURN;
        END

        SELECT TOP 1
            @IdInstalacion = IdInstalacion,
            @EstadoInstalacion = Estado,
            @FotoEvidencia = FotoEvidencia,
            @PruebaVelocidad = PruebaVelocidad,
            @FechaRealizacion = FechaRealizacion
        FROM dbo.InstalacionesProgramadas
        WHERE IdSolicitud = @IdSolicitud
        ORDER BY IdInstalacion DESC;

        IF @IdInstalacion IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -5 AS Resultado, 0 AS IdGenerado, 'La solicitud no tiene instalación programada.' AS Mensaje;
            RETURN;
        END

        IF @EstadoInstalacion <> 'Realizada'
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -6 AS Resultado, 0 AS IdGenerado, 'La instalación aún no está realizada.' AS Mensaje;
            RETURN;
        END

        IF @FechaRealizacion IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -7 AS Resultado, 0 AS IdGenerado, 'La instalación no tiene fecha de realización.' AS Mensaje;
            RETURN;
        END

        IF @FotoEvidencia IS NULL OR LTRIM(RTRIM(@FotoEvidencia)) = ''
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -8 AS Resultado, 0 AS IdGenerado, 'Falta la foto de evidencia.' AS Mensaje;
            RETURN;
        END

        IF @PruebaVelocidad IS NULL OR LTRIM(RTRIM(@PruebaVelocidad)) = ''
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -9 AS Resultado, 0 AS IdGenerado, 'Falta la prueba de velocidad.' AS Mensaje;
            RETURN;
        END

        IF EXISTS (
            SELECT 1
            FROM dbo.Clientes
            WHERE IdSolicitudOrigen = @IdSolicitud
              AND Activo = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -10 AS Resultado, 0 AS IdGenerado, 'Esta solicitud ya fue convertida en cliente.' AS Mensaje;
            RETURN;
        END

        IF EXISTS (
            SELECT 1
            FROM dbo.Clientes
            WHERE Telefono = @Telefono
              AND Activo = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -4 AS Resultado, 0 AS IdGenerado, 'Ya existe un cliente activo con ese teléfono.' AS Mensaje;
            RETURN;
        END

        SELECT 
            @Precio = Precio,
            @Porcentaje = PorcentajeDistribucion
        FROM dbo.PaquetesServicio
        WHERE IdPaquete = @IdPaquete;

        IF @Precio IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -11 AS Resultado, 0 AS IdGenerado, 'El paquete no existe o no tiene precio.' AS Mensaje;
            RETURN;
        END

        INSERT INTO dbo.Clientes (
            NombreCompleto,
            Telefono,
            Correo,
            Direccion,
            EstadoPago,
            FechaRegistro,
            IdVendedor,
            IdSolicitudOrigen,
            Activo
        )
        VALUES (
            @NombreCompleto,
            @Telefono,
            @Correo,
            @Direccion,
            'Pendiente',
            GETUTCDATE(),
            @IdVendedor,
            @IdSolicitud,
            1
        );

        SET @IdCliente = SCOPE_IDENTITY();

        INSERT INTO dbo.AsignacionesPaquete (
            IdCliente,
            IdPaquete,
            IdVendedor,
            FechaAsignacion,
            Estado
        )
        VALUES (
            @IdCliente,
            @IdPaquete,
            @IdVendedor,
            GETUTCDATE(),
            'Activa'
        );

        SET @IdAsignacion = SCOPE_IDENTITY();

        INSERT INTO dbo.Ventas (
            IdAsignacion,
            FechaVenta,
            Monto,
            Estado,
            PorcentajeDistribucion
        )
        VALUES (
            @IdAsignacion,
            GETUTCDATE(),
            @Precio,
            'Activa',
            @Porcentaje
        );

        UPDATE dbo.SolicitudesServicio
        SET 
            Estado = 'Convertida',
            IdVendedorAsignado = @IdVendedor,
            Notas = COALESCE(@Notas, 'Solicitud convertida en cliente activo.')
        WHERE IdSolicitud = @IdSolicitud;

        COMMIT TRANSACTION;

        SELECT 
            1 AS Resultado, 
            @IdCliente AS IdGenerado, 
            'Cliente activado correctamente.' AS Mensaje;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            -99 AS Resultado, 
            0 AS IdGenerado, 
            ERROR_MESSAGE() AS Mensaje;
    END CATCH
END;
GO



DROP PROCEDURE IF EXISTS dbo.listarInstalacionesGenerales;
GO
CREATE OR ALTER PROCEDURE dbo.listarInstalacionesGenerales
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.IdInstalacion,
        i.IdSolicitud,

        s.Estado AS EstadoSolicitud,

        i.IdTecnicoAsignado,
        tecnico.Nombre AS NombreTecnico,

        i.IdVendedorPrograma,
        vendedor.Nombre AS NombreVendedor,

        i.FechaProgramada,
        i.FechaRealizacion,

        i.UbicacionInstalacion,
        i.FotoEvidencia,
        i.PruebaVelocidad,

        i.Estado,
        i.NotasVendedor,
        i.ComentarioTecnico,

        ce.NombreCompleto AS NombreCliente,
        ce.NombreCompleto,
        ce.Telefono,
        ce.Correo,
        ce.Direccion,

        p.Nombre AS NombrePaquete

    FROM dbo.InstalacionesProgramadas i
    INNER JOIN dbo.SolicitudesServicio s
        ON i.IdSolicitud = s.IdSolicitud

    INNER JOIN dbo.ClientesExternos ce
        ON s.IdClienteExterno = ce.IdClienteExterno

    LEFT JOIN dbo.PaquetesServicio p
        ON s.IdPaquete = p.IdPaquete

    INNER JOIN dbo.Usuarios tecnico
        ON i.IdTecnicoAsignado = tecnico.IdUsuario

    INNER JOIN dbo.Usuarios vendedor
        ON i.IdVendedorPrograma = vendedor.IdUsuario

    ORDER BY 
        i.FechaProgramada DESC,
        i.IdInstalacion DESC;
END;
GO



----------------------------
--Apartado de ingresos
DROP PROCEDURE IF EXISTS dbo.listarClientesParaIngreso;
GO

CREATE OR ALTER PROCEDURE dbo.listarClientesParaIngreso
AS
BEGIN
    SET NOCOUNT ON;
	EXEC dbo.actualizarEstadosPagoClientes;
    SELECT
        c.IdCliente,
        c.NombreCompleto,
        c.Telefono,
        c.Correo,
        c.EstadoPago,

        ap.IdAsignacion,
        ap.FechaVencimiento,
        ap.Estado AS EstadoAsignacion,

        p.IdPaquete,
        p.Nombre AS NombrePaquete,
        p.Velocidad,
        p.Precio

    FROM dbo.Clientes c
    INNER JOIN dbo.AsignacionesPaquete ap
        ON c.IdCliente = ap.IdCliente
       AND ap.Estado = 'Activa'

    INNER JOIN dbo.PaquetesServicio p
        ON ap.IdPaquete = p.IdPaquete

    WHERE c.Activo = 1
      AND c.EstadoPago = 'Pendiente'

    ORDER BY c.NombreCompleto;
END;
GO



CREATE OR ALTER PROCEDURE dbo.actualizarEstadosPagoClientes
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE c
    SET c.EstadoPago = 'Pendiente'
    FROM dbo.Clientes c
    INNER JOIN dbo.AsignacionesPaquete ap
        ON c.IdCliente = ap.IdCliente
       AND ap.Estado = 'Activa'
    WHERE c.Activo = 1
      AND c.EstadoPago = 'AlDia'
      AND CAST(ap.FechaVencimiento AS DATE) <= CAST(GETDATE() AS DATE);
END;
GO




CREATE OR ALTER PROCEDURE dbo.registrarIngresoCliente
    @IdCliente INT,
    @Monto DECIMAL(10,2),
    @MetodoPago NVARCHAR(50),
    @ReferenciaPago NVARCHAR(100) = NULL,
    @Descripcion NVARCHAR(300) = NULL,
    @IdRegistradoPor INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE
            @IdPaquete INT,
            @IdAsignacion INT,
            @Precio DECIMAL(10,2),
            @FechaPago DATETIME2,
            @FechaVencimientoActual DATETIME2,
            @FechaProximoPago DATETIME2,
            @IdIngreso INT;

        SET @FechaPago = GETDATE();

        SELECT TOP 1
            @IdAsignacion = ap.IdAsignacion,
            @IdPaquete = ap.IdPaquete,
            @FechaVencimientoActual = ap.FechaVencimiento,
            @Precio = p.Precio
        FROM dbo.AsignacionesPaquete ap
        INNER JOIN dbo.PaquetesServicio p
            ON ap.IdPaquete = p.IdPaquete
        WHERE ap.IdCliente = @IdCliente
          AND ap.Estado = 'Activa'
        ORDER BY ap.IdAsignacion DESC;

        IF @IdAsignacion IS NULL
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -1 AS Resultado,
                0 AS IdGenerado,
                'El cliente no tiene un paquete activo asignado.' AS Mensaje,
                NULL AS FechaProximoPago;

            RETURN;
        END

        IF @Monto <= 0
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -2 AS Resultado,
                0 AS IdGenerado,
                'El monto debe ser mayor a cero.' AS Mensaje,
                NULL AS FechaProximoPago;

            RETURN;
        END

        IF @MetodoPago IS NULL OR LTRIM(RTRIM(@MetodoPago)) = ''
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 
                -3 AS Resultado,
                0 AS IdGenerado,
                'Debe seleccionar un método de pago.' AS Mensaje,
                NULL AS FechaProximoPago;

            RETURN;
        END

        -- Si el cliente paga antes de vencer, se suma un mes a la fecha actual de vencimiento.
        -- Si ya venció o no tiene fecha, se suma un mes desde hoy.
        IF @FechaVencimientoActual IS NOT NULL 
           AND CAST(@FechaVencimientoActual AS DATE) >= CAST(@FechaPago AS DATE)
        BEGIN
            SET @FechaProximoPago = DATEADD(MONTH, 1, @FechaVencimientoActual);
        END
        ELSE
        BEGIN
            SET @FechaProximoPago = DATEADD(MONTH, 1, @FechaPago);
        END

        INSERT INTO dbo.Ingresos
        (
            IdCliente,
            IdPaquete,
            Monto,
            Fecha,
            Descripcion,
            IdRegistradoPor,
            TipoIngreso,
            MetodoPago,
            Estado,
            ReferenciaPago,
            FechaProximoPago
        )
        VALUES
        (
            @IdCliente,
            @IdPaquete,
            @Monto,
            @FechaPago,
            COALESCE(@Descripcion, 'Pago mensual de servicio'),
            @IdRegistradoPor,
            'Mensualidad',
            @MetodoPago,
            'Registrado',
            @ReferenciaPago,
            @FechaProximoPago
        );

        SET @IdIngreso = SCOPE_IDENTITY();

        UPDATE dbo.Clientes
        SET EstadoPago = 'AlDia'
        WHERE IdCliente = @IdCliente;

        UPDATE dbo.AsignacionesPaquete
        SET FechaVencimiento = @FechaProximoPago
        WHERE IdAsignacion = @IdAsignacion;

        COMMIT TRANSACTION;

        SELECT 
            1 AS Resultado,
            @IdIngreso AS IdGenerado,
            'Ingreso registrado correctamente.' AS Mensaje,
            @FechaProximoPago AS FechaProximoPago;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            -99 AS Resultado,
            0 AS IdGenerado,
            ERROR_MESSAGE() AS Mensaje,
            NULL AS FechaProximoPago;

    END CATCH
END;
GO





CREATE OR ALTER PROCEDURE dbo.listarIngresos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.IdIngreso,
        i.IdCliente,
        c.NombreCompleto AS NombreCliente,
        c.EstadoPago AS EstadoPago,

        i.IdPaquete,
        p.Nombre AS NombrePaquete,
        p.Velocidad,

        i.Monto,
        i.Fecha,
        i.Descripcion,
        i.TipoIngreso,
        i.MetodoPago,
        i.Estado,
        i.ReferenciaPago,
        i.FechaProximoPago,

        i.IdRegistradoPor,
        u.Nombre AS NombreRegistradoPor

    FROM dbo.Ingresos i

    LEFT JOIN dbo.Clientes c
        ON i.IdCliente = c.IdCliente

    LEFT JOIN dbo.PaquetesServicio p
        ON i.IdPaquete = p.IdPaquete

    INNER JOIN dbo.Usuarios u
        ON i.IdRegistradoPor = u.IdUsuario

    ORDER BY i.Fecha DESC, i.IdIngreso DESC;
END;
GO

CREATE OR ALTER PROCEDURE validarDesactivacionUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Detalles NVARCHAR(2000) = N'';
    DECLARE @VentasActivas INT = 0;
    DECLARE @SolicitudesEnProceso INT = 0;
    DECLARE @InstalacionesVendedor INT = 0;
    DECLARE @InstalacionesTecnico INT = 0;
    DECLARE @TicketsAbiertos INT = 0;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT 0 AS TieneActividadPendiente, N'' AS Detalles;
        RETURN;
    END

    SELECT @VentasActivas = COUNT(*)
    FROM Ventas v
    INNER JOIN AsignacionesPaquete ap ON v.IdAsignacion = ap.IdAsignacion
    WHERE ap.IdVendedor = @IdUsuario
      AND v.Estado = 'Activa'
      AND ap.Estado = 'Activa';

    SELECT @SolicitudesEnProceso = COUNT(*)
    FROM SolicitudesServicio
    WHERE IdVendedorAsignado = @IdUsuario
      AND Estado IN ('Pendiente', 'Atendida', 'Programada');

    SELECT @InstalacionesVendedor = COUNT(*)
    FROM InstalacionesProgramadas
    WHERE IdVendedorPrograma = @IdUsuario
      AND Estado IN ('Programada', 'Reprogramada');

    SELECT @InstalacionesTecnico = COUNT(*)
    FROM InstalacionesProgramadas
    WHERE IdTecnicoAsignado = @IdUsuario
      AND Estado IN ('Programada', 'Reprogramada');

    SELECT @TicketsAbiertos = COUNT(*)
    FROM Tickets
    WHERE IdAsignadoA = @IdUsuario
      AND Estado IN ('Pendiente', 'EnProceso');

    IF @VentasActivas > 0
        SET @Detalles = @Detalles + CAST(@VentasActivas AS NVARCHAR(10)) + N' venta(s) activa(s)|';

    IF @SolicitudesEnProceso > 0
        SET @Detalles = @Detalles + CAST(@SolicitudesEnProceso AS NVARCHAR(10)) + N' solicitud(es) en proceso|';

    IF @InstalacionesVendedor > 0
        SET @Detalles = @Detalles + CAST(@InstalacionesVendedor AS NVARCHAR(10)) + N' instalación(es) programada(s) como vendedor|';

    IF @InstalacionesTecnico > 0
        SET @Detalles = @Detalles + CAST(@InstalacionesTecnico AS NVARCHAR(10)) + N' instalación(es) pendiente(s) como técnico|';

    IF @TicketsAbiertos > 0
        SET @Detalles = @Detalles + CAST(@TicketsAbiertos AS NVARCHAR(10)) + N' ticket(s) abierto(s)|';

    IF LEN(@Detalles) > 0
        SET @Detalles = LEFT(@Detalles, LEN(@Detalles) - 1);

    SELECT
        CASE
            WHEN @VentasActivas + @SolicitudesEnProceso + @InstalacionesVendedor + @InstalacionesTecnico + @TicketsAbiertos > 0
            THEN 1 ELSE 0
        END AS TieneActividadPendiente,
        @Detalles AS Detalles;
END;
GO