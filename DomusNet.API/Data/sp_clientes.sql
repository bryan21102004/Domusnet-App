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
