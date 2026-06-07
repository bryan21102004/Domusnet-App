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
