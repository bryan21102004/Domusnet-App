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
