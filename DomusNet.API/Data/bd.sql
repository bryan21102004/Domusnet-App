
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

