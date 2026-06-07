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
