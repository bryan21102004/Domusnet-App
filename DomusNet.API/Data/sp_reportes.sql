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
