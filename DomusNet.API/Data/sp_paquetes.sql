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
