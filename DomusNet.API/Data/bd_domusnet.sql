-- =============================================
-- FILE: bd.sql
-- =============================================


CREATE DATABASE DomusNet;
GO
USE DomusNet;
GO

-- ────────────────────────────────────────────────────
--  ROLES
-- ────────────────────────────────────────────────────
CREATE TABLE Roles (
    IdRol           INT           NOT NULL IDENTITY(1,1),
    NombreRol       NVARCHAR(50)  NOT NULL,
    Descripcion     NVARCHAR(200) NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (IdRol),
    CONSTRAINT UQ_Roles_Nombre UNIQUE (NombreRol)
);

INSERT INTO Roles (NombreRol, Descripcion) VALUES
  ('Administrador', 'Control total del sistema'),
  ('Vendedor',      'Gestión de clientes y paquetes'),
  ('Tecnico',       'Atención de tickets e incidencias');

-- ────────────────────────────────────────────────────
--  USUARIOS  (Administrador / Vendedor / Tecnico)
-- ────────────────────────────────────────────────────
CREATE TABLE Usuarios (
    IdUsuario       INT            NOT NULL IDENTITY(1,1),
    Nombre          NVARCHAR(100)  NOT NULL,
    Correo          NVARCHAR(150)  NOT NULL,
    Contrasena      NVARCHAR(255)  NOT NULL,  -- bcrypt hash
    Telefono        NVARCHAR(20)   NULL,
    IdRol           INT            NOT NULL,
    Activo          BIT            NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME2      NOT NULL DEFAULT GETDATE(),
    FechaModificacion DATETIME2    NULL,
    RefreshToken      NVARCHAR(255) NULL,
    CONSTRAINT PK_Usuarios  PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo),
    CONSTRAINT FK_Usuarios_Roles  FOREIGN KEY (IdRol)
        REFERENCES Roles (IdRol)
);

-- ────────────────────────────────────────────────────
--  PAQUETES DE SERVICIO
-- ────────────────────────────────────────────────────
CREATE TABLE PaquetesServicio (
    IdPaquete       INT            NOT NULL IDENTITY(1,1),
    Nombre          NVARCHAR(100)  NOT NULL,
    Descripcion     NVARCHAR(500)  NULL,
    Velocidad       NVARCHAR(50)   NULL,       -- ej. "50 Mbps"
    Precio          DECIMAL(10,2)  NOT NULL,
    PorcentajeDistribucion DECIMAL(5,2) NULL,  -- para reparto de ingresos
    Estado          NVARCHAR(20)   NOT NULL DEFAULT 'Activo'
        CONSTRAINT CK_Paquetes_Estado CHECK (Estado IN ('Activo','Inactivo')),
    FechaCreacion   DATETIME2      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_PaquetesServicio PRIMARY KEY (IdPaquete)
);

-- ────────────────────────────────────────────────────
--  CLIENTES EXTERNOS (área pública, sin auth)
-- ────────────────────────────────────────────────────
CREATE TABLE ClientesExternos (
    IdClienteExterno INT           NOT NULL IDENTITY(1,1),
    NombreCompleto   NVARCHAR(150) NOT NULL,
    Telefono         NVARCHAR(20)  NOT NULL,
    Correo           NVARCHAR(150) NULL,
    Direccion        NVARCHAR(300) NOT NULL,
    CONSTRAINT PK_ClientesExternos PRIMARY KEY (IdClienteExterno)
);

-- ────────────────────────────────────────────────────
--  SOLICITUDES DE SERVICIO (formulario público → WhatsApp)
-- ────────────────────────────────────────────────────
CREATE TABLE SolicitudesServicio (
    IdSolicitud      INT           NOT NULL IDENTITY(1,1),
    IdClienteExterno INT           NOT NULL,
    IdPaquete        INT           NULL,       -- paquete de interés
    FechaSolicitud   DATETIME2     NOT NULL DEFAULT GETDATE(),
    Estado           NVARCHAR(30)  NOT NULL DEFAULT 'Pendiente'
        CONSTRAINT CK_Solicitudes_Estado CHECK (Estado IN ('Pendiente','Atendida','Cancelada')),
    IdVendedorAsignado INT         NULL,       -- se asigna tras contacto
    Notas            NVARCHAR(500) NULL,
    CONSTRAINT PK_SolicitudesServicio   PRIMARY KEY (IdSolicitud),
    CONSTRAINT FK_Solicitudes_Cliente   FOREIGN KEY (IdClienteExterno)
        REFERENCES ClientesExternos (IdClienteExterno),
    CONSTRAINT FK_Solicitudes_Paquete   FOREIGN KEY (IdPaquete)
        REFERENCES PaquetesServicio (IdPaquete),
    CONSTRAINT FK_Solicitudes_Vendedor  FOREIGN KEY (IdVendedorAsignado)
        REFERENCES Usuarios (IdUsuario)
);

-- ────────────────────────────────────────────────────
--  CLIENTES ACTIVOS (registrados por vendedor tras instalación)
-- ────────────────────────────────────────────────────
CREATE TABLE Clientes (
    IdCliente        INT           NOT NULL IDENTITY(1,1),
    NombreCompleto   NVARCHAR(150) NOT NULL,
    Telefono         NVARCHAR(20)  NOT NULL,
    Correo           NVARCHAR(150) NULL,
    Direccion        NVARCHAR(300) NOT NULL,
    EstadoPago       NVARCHAR(20)  NOT NULL DEFAULT 'AlDia'
        CONSTRAINT CK_Clientes_EstadoPago CHECK (EstadoPago IN ('AlDia','Moroso','Pendiente')),
    FechaRegistro    DATETIME2     NOT NULL DEFAULT GETDATE(),
    IdVendedor       INT           NOT NULL,   -- vendedor que registró
    IdSolicitudOrigen INT          NULL,       -- trazabilidad desde solicitud
    Activo           BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_Clientes              PRIMARY KEY (IdCliente),
    CONSTRAINT FK_Clientes_Vendedor     FOREIGN KEY (IdVendedor)
        REFERENCES Usuarios (IdUsuario),
    CONSTRAINT FK_Clientes_Solicitud    FOREIGN KEY (IdSolicitudOrigen)
        REFERENCES SolicitudesServicio (IdSolicitud)
);

-- ────────────────────────────────────────────────────
--  ASIGNACIONES DE PAQUETE  (cliente ↔ paquete)
-- ────────────────────────────────────────────────────
CREATE TABLE AsignacionesPaquete (
    IdAsignacion     INT           NOT NULL IDENTITY(1,1),
    IdCliente        INT           NOT NULL,
    IdPaquete        INT           NOT NULL,
    IdVendedor       INT           NOT NULL,
    FechaAsignacion  DATETIME2     NOT NULL DEFAULT GETDATE(),
    FechaVencimiento DATETIME2     NULL,
    Estado           NVARCHAR(20)  NOT NULL DEFAULT 'Activa'
        CONSTRAINT CK_Asignaciones_Estado CHECK (Estado IN ('Activa','Suspendida','Cancelada')),
    CONSTRAINT PK_AsignacionesPaquete   PRIMARY KEY (IdAsignacion),
    CONSTRAINT FK_Asig_Cliente          FOREIGN KEY (IdCliente)
        REFERENCES Clientes (IdCliente),
    CONSTRAINT FK_Asig_Paquete          FOREIGN KEY (IdPaquete)
        REFERENCES PaquetesServicio (IdPaquete),
    CONSTRAINT FK_Asig_Vendedor         FOREIGN KEY (IdVendedor)
        REFERENCES Usuarios (IdUsuario)
);

-- ────────────────────────────────────────────────────
--  TICKETS  (averías, avisos, recordatorios)
-- ────────────────────────────────────────────────────
CREATE TABLE Tickets (
    IdTicket         INT           NOT NULL IDENTITY(1,1),
    Titulo           NVARCHAR(200) NOT NULL,
    Descripcion      NVARCHAR(1000) NULL,
    Tipo             NVARCHAR(20)  NOT NULL
        CONSTRAINT CK_Tickets_Tipo CHECK (Tipo IN ('Averia','Aviso','Recordatorio')),
    Estado           NVARCHAR(20)  NOT NULL DEFAULT 'Pendiente'
        CONSTRAINT CK_Tickets_Estado CHECK (Estado IN ('Pendiente','EnProceso','Resuelto')),
    Prioridad        NVARCHAR(10)  NOT NULL DEFAULT 'Media'
        CONSTRAINT CK_Tickets_Prioridad CHECK (Prioridad IN ('Alta','Media','Baja')),
    IdCliente        INT           NULL,       -- NULL si es ticket global
    IdCreadoPor      INT           NOT NULL,   -- Admin o Técnico
    IdAsignadoA      INT           NULL,       -- Técnico asignado
    FechaCreacion    DATETIME2     NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME2   NULL,
    FechaCierre      DATETIME2     NULL,
    EsGlobal         BIT           NOT NULL DEFAULT 0,  -- avería global visible en área pública
    CONSTRAINT PK_Tickets            PRIMARY KEY (IdTicket),
    CONSTRAINT FK_Tickets_Cliente    FOREIGN KEY (IdCliente)
        REFERENCES Clientes (IdCliente),
    CONSTRAINT FK_Tickets_CreadoPor  FOREIGN KEY (IdCreadoPor)
        REFERENCES Usuarios (IdUsuario),
    CONSTRAINT FK_Tickets_AsignadoA  FOREIGN KEY (IdAsignadoA)
        REFERENCES Usuarios (IdUsuario)
);

-- ────────────────────────────────────────────────────
--  HISTORIAL DE ESTADOS DE TICKET  (trazabilidad)
-- ────────────────────────────────────────────────────
CREATE TABLE HistorialTickets (
    IdHistorial      INT           NOT NULL IDENTITY(1,1),
    IdTicket         INT           NOT NULL,
    EstadoAnterior   NVARCHAR(20)  NULL,
    EstadoNuevo      NVARCHAR(20)  NOT NULL,
    Comentario       NVARCHAR(500) NULL,
    IdUsuario        INT           NOT NULL,   -- quién hizo el cambio
    Fecha            DATETIME2     NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_HistorialTickets   PRIMARY KEY (IdHistorial),
    CONSTRAINT FK_Historial_Ticket   FOREIGN KEY (IdTicket)
        REFERENCES Tickets (IdTicket),
    CONSTRAINT FK_Historial_Usuario  FOREIGN KEY (IdUsuario)
        REFERENCES Usuarios (IdUsuario)
);

-- ────────────────────────────────────────────────────
--  NOTIFICACIONES
-- ────────────────────────────────────────────────────
CREATE TABLE Notificaciones (
    IdNotificacion   INT           NOT NULL IDENTITY(1,1),
    IdUsuarioDestino INT           NOT NULL,
    IdTicket         INT           NULL,
    Mensaje          NVARCHAR(500) NOT NULL,
    Leida            BIT           NOT NULL DEFAULT 0,
    FechaEnvio       DATETIME2     NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Notificaciones        PRIMARY KEY (IdNotificacion),
    CONSTRAINT FK_Noti_UsuarioDestino   FOREIGN KEY (IdUsuarioDestino)
        REFERENCES Usuarios (IdUsuario),
    CONSTRAINT FK_Noti_Ticket           FOREIGN KEY (IdTicket)
        REFERENCES Tickets (IdTicket)
);

-- ────────────────────────────────────────────────────
--  INGRESOS  (registrados por Administrador)
-- ────────────────────────────────────────────────────
CREATE TABLE Ingresos (
    IdIngreso        INT           NOT NULL IDENTITY(1,1),
    IdCliente        INT           NULL,
    IdPaquete        INT           NULL,
    Monto            DECIMAL(10,2) NOT NULL,
    Fecha            DATETIME2     NOT NULL DEFAULT GETDATE(),
    Descripcion      NVARCHAR(300) NULL,
    IdRegistradoPor  INT           NOT NULL,
    CONSTRAINT PK_Ingresos              PRIMARY KEY (IdIngreso),
    CONSTRAINT FK_Ingresos_Cliente      FOREIGN KEY (IdCliente)
        REFERENCES Clientes (IdCliente),
    CONSTRAINT FK_Ingresos_Paquete      FOREIGN KEY (IdPaquete)
        REFERENCES PaquetesServicio (IdPaquete),
    CONSTRAINT FK_Ingresos_Registrado   FOREIGN KEY (IdRegistradoPor)
        REFERENCES Usuarios (IdUsuario)
);

-- ────────────────────────────────────────────────────
--  VENTAS  (generadas al asignar un paquete)
-- ────────────────────────────────────────────────────
CREATE TABLE Ventas (
    IdVenta          INT           NOT NULL IDENTITY(1,1),
    IdAsignacion     INT           NOT NULL,
    FechaVenta       DATETIME2     NOT NULL DEFAULT GETDATE(),
    Monto            DECIMAL(10,2) NOT NULL,
    Estado           NVARCHAR(20)  NOT NULL DEFAULT 'Activa',
    PorcentajeDistribucion DECIMAL(5,2) NULL,
    CONSTRAINT PK_Ventas             PRIMARY KEY (IdVenta),
    CONSTRAINT FK_Ventas_Asignacion  FOREIGN KEY (IdAsignacion)
        REFERENCES AsignacionesPaquete (IdAsignacion)
);

-- ────────────────────────────────────────────────────
--  REPORTES
-- ────────────────────────────────────────────────────
CREATE TABLE Reportes (
    IdReporte        INT           NOT NULL IDENTITY(1,1),
    Tipo             NVARCHAR(30)  NOT NULL
        CONSTRAINT CK_Reportes_Tipo CHECK (Tipo IN ('Ingresos','Clientes','Tickets','Operativo')),
    Parametros       NVARCHAR(1000) NULL,      -- JSON con filtros aplicados
    FechaGeneracion  DATETIME2     NOT NULL DEFAULT GETDATE(),
    IdGeneradoPor    INT           NOT NULL,
    CONSTRAINT PK_Reportes           PRIMARY KEY (IdReporte),
    CONSTRAINT FK_Reportes_Usuario   FOREIGN KEY (IdGeneradoPor)
        REFERENCES Usuarios (IdUsuario)
);
CREATE TABLE AuditoriaAcciones (
    IdAuditoriaAccion INT            NOT NULL IDENTITY(1,1),
    Tabla             NVARCHAR(100)  NOT NULL,
    IdRegistro        INT            NULL,
    Accion            NVARCHAR(50)   NOT NULL,
    IdUsuario         INT            NOT NULL,
    Fecha             DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    Detalle           NVARCHAR(1000) NULL,
    CONSTRAINT PK_AuditoriaAcciones      PRIMARY KEY (IdAuditoriaAccion),
    CONSTRAINT FK_AuditoriaAcciones_User FOREIGN KEY (IdUsuario)
        REFERENCES Usuarios (IdUsuario)
);





-- =============================================
-- FILE: sp_auth.sql
-- =============================================

USE DomusNet;
GO

-- Agregar columna RefreshToken si no existe (ejecutar una sola vez)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Usuarios') AND name = 'RefreshToken'
)
BEGIN
    ALTER TABLE Usuarios ADD RefreshToken NVARCHAR(255) NULL;
END
GO

-- ── AUTH / TOKENS ────────────────────────────────────────

CREATE OR ALTER PROCEDURE modificarToken
    @IdUsuario      INT,
    @RefreshToken   NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuarios
    SET RefreshToken = @RefreshToken,
        FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE verificarTokenR
    @IdUsuario      INT,
    @RefreshToken   NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdRol
    FROM Usuarios
    WHERE IdUsuario = @IdUsuario
      AND RefreshToken = @RefreshToken
      AND Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE buscarUsuarioLogin
    @Correo NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Correo, u.Contrasena, u.Telefono,
           u.IdRol, u.Activo, r.NombreRol
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.Correo = @Correo AND u.Activo = 1;
END
GO




-- =============================================
-- FILE: sp_clientes.sql
-- =============================================

USE DomusNet;
GO

CREATE OR ALTER PROCEDURE listarClientes
    @EstadoPago NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdCliente, c.NombreCompleto, c.Telefono, c.Correo, c.Direccion,
           c.EstadoPago, c.FechaRegistro, c.IdVendedor, u.Nombre AS NombreVendedor, c.Activo
    FROM Clientes c
    INNER JOIN Usuarios u ON c.IdVendedor = u.IdUsuario
    WHERE (@EstadoPago IS NULL OR c.EstadoPago = @EstadoPago)
      AND c.Activo = 1
    ORDER BY c.NombreCompleto;
END
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

    SELECT 1 AS Resultado, @IdTicket AS IdGenerado;
END
GO

-- Retorna: 1=ok, 0=no existe
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

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo)
    BEGIN
        SELECT -1 AS Resultado, 0 AS IdGenerado;
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

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND IdUsuario <> @IdUsuario)
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

-- Paquetes de ejemplo (para que /api/paquetes no devuelva array vacio)
IF NOT EXISTS (SELECT 1 FROM PaquetesServicio)
BEGIN
    INSERT INTO PaquetesServicio (Nombre, Descripcion, Velocidad, Precio, PorcentajeDistribucion, Estado, FechaCreacion)
    VALUES
        ('Plan Basico 25 Mbps',  'Internet basico',  '25 Mbps',  10000, 10, 'Activo', GETUTCDATE()),
        ('Plan Familiar 50 Mbps','Internet familiar', '50 Mbps',  15000, 10, 'Activo', GETUTCDATE()),
        ('Plan Premium 100 Mbps','Internet premium',  '100 Mbps', 25000, 10, 'Activo', GETUTCDATE());
    PRINT 'Paquetes de ejemplo insertados.';
END
GO



