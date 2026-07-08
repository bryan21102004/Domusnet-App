USE Domusnet;
GO


CREATE TABLE dbo.ConfiguracionDistribucionIngresos (
    IdConfiguracion INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,

    PorcentajeDomusNet DECIMAL(5,2) NOT NULL,
    PorcentajeTrabajadores DECIMAL(5,2) NOT NULL,

    PorcentajeIVA DECIMAL(5,2) NOT NULL DEFAULT 13.00,
    PorcentajeCruzRoja DECIMAL(5,2) NOT NULL DEFAULT 1.00,
    Porcentaje911 DECIMAL(5,2) NOT NULL DEFAULT 0.75,

    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
    IdCreadoPor INT NOT NULL,

    CONSTRAINT FK_ConfigDistribucion_Usuario
        FOREIGN KEY (IdCreadoPor)
        REFERENCES dbo.Usuarios(IdUsuario)
);
GO


CREATE TABLE dbo.ConfiguracionDistribucionTrabajadores (
    IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdConfiguracion INT NOT NULL,
    IdUsuario INT NOT NULL,
    Porcentaje DECIMAL(5,2) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_ConfigTrabajadores_Config
        FOREIGN KEY (IdConfiguracion)
        REFERENCES dbo.ConfiguracionDistribucionIngresos(IdConfiguracion),

    CONSTRAINT FK_ConfigTrabajadores_Usuario
        FOREIGN KEY (IdUsuario)
        REFERENCES dbo.Usuarios(IdUsuario)
);
GO



CREATE TABLE dbo.IngresosMensuales (
    IdIngresoMensual INT IDENTITY(1,1) PRIMARY KEY,

    Mes INT NOT NULL,
    Anio INT NOT NULL,
    Quincena NVARCHAR(20) NULL,

    MontoTotalBruto DECIMAL(10,2) NOT NULL,
    MontoTotalConRebajas DECIMAL(10,2) NOT NULL,

    PorcentajeDomusNet DECIMAL(5,2) NOT NULL,
    MontoDomusNet DECIMAL(10,2) NOT NULL,

    PorcentajeTrabajadores DECIMAL(5,2) NOT NULL,
    MontoTrabajadores DECIMAL(10,2) NOT NULL,

    PorcentajeIVA DECIMAL(5,2) NOT NULL,
    PorcentajeCruzRoja DECIMAL(5,2) NOT NULL,
    Porcentaje911 DECIMAL(5,2) NOT NULL,

    IdConfiguracion INT NOT NULL,
    IdRegistradoPor INT NOT NULL,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETDATE(),
    Notas NVARCHAR(500) NULL,

    CONSTRAINT FK_IngresosMensuales_Config
        FOREIGN KEY (IdConfiguracion)
        REFERENCES dbo.ConfiguracionDistribucionIngresos(IdConfiguracion),

    CONSTRAINT FK_IngresosMensuales_Usuario
        FOREIGN KEY (IdRegistradoPor)
        REFERENCES dbo.Usuarios(IdUsuario)
);
GO





CREATE TABLE dbo.DistribucionIngresosMensuales (
    IdDistribucion INT IDENTITY(1,1) PRIMARY KEY,

    IdIngresoMensual INT NOT NULL,
    IdUsuario INT NOT NULL,

    PorcentajeAplicado DECIMAL(5,2) NOT NULL,
    MontoAsignado DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_DistribucionMensual_Ingreso
        FOREIGN KEY (IdIngresoMensual)
        REFERENCES dbo.IngresosMensuales(IdIngresoMensual),

    CONSTRAINT FK_DistribucionMensual_Usuario
        FOREIGN KEY (IdUsuario)
        REFERENCES dbo.Usuarios(IdUsuario)
);
GO




CREATE OR ALTER PROCEDURE dbo.guardarConfiguracionDistribucion
    @Nombre NVARCHAR(100),
    @PorcentajeDomusNet DECIMAL(5,2),
    @PorcentajeIVA DECIMAL(5,2),
    @PorcentajeCruzRoja DECIMAL(5,2),
    @Porcentaje911 DECIMAL(5,2),
    @IdCreadoPor INT,
    @TrabajadoresJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE 
            @IdConfiguracion INT,
            @PorcentajeTrabajadores DECIMAL(5,2),
            @SumaTrabajadores DECIMAL(5,2);

        SET @PorcentajeTrabajadores = 100 - @PorcentajeDomusNet;

        IF @PorcentajeDomusNet < 0 OR @PorcentajeDomusNet > 100
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1 AS Resultado, 'El porcentaje de DomusNet no es válido.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        SELECT @SumaTrabajadores = SUM(Porcentaje)
        FROM OPENJSON(@TrabajadoresJson)
        WITH (
            IdUsuario INT '$.idUsuario',
            Porcentaje DECIMAL(5,2) '$.porcentaje'
        );

        IF ISNULL(@SumaTrabajadores, 0) <> @PorcentajeTrabajadores
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -2 AS Resultado, 'La suma de porcentajes de trabajadores no coincide con el porcentaje restante.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        UPDATE dbo.ConfiguracionDistribucionIngresos
        SET Activo = 0
        WHERE Activo = 1;

        INSERT INTO dbo.ConfiguracionDistribucionIngresos (
            Nombre,
            PorcentajeDomusNet,
            PorcentajeTrabajadores,
            PorcentajeIVA,
            PorcentajeCruzRoja,
            Porcentaje911,
            Activo,
            IdCreadoPor
        )
        VALUES (
            @Nombre,
            @PorcentajeDomusNet,
            @PorcentajeTrabajadores,
            @PorcentajeIVA,
            @PorcentajeCruzRoja,
            @Porcentaje911,
            1,
            @IdCreadoPor
        );

        SET @IdConfiguracion = SCOPE_IDENTITY();

        INSERT INTO dbo.ConfiguracionDistribucionTrabajadores (
            IdConfiguracion,
            IdUsuario,
            Porcentaje,
            Activo
        )
        SELECT
            @IdConfiguracion,
            IdUsuario,
            Porcentaje,
            1
        FROM OPENJSON(@TrabajadoresJson)
        WITH (
            IdUsuario INT '$.idUsuario',
            Porcentaje DECIMAL(5,2) '$.porcentaje'
        );

        COMMIT TRANSACTION;

        SELECT 
            1 AS Resultado,
            'Configuración guardada correctamente.' AS Mensaje,
            @IdConfiguracion AS IdGenerado;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            -99 AS Resultado,
            ERROR_MESSAGE() AS Mensaje,
            0 AS IdGenerado;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.generarReporteIngresosMensual
    @Mes INT,
    @Anio INT,
    @Quincena NVARCHAR(20) = NULL,
    @IdRegistradoPor INT,
    @Notas NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE
            @IdConfiguracion INT,
            @PorcentajeDomusNet DECIMAL(5,2),
            @PorcentajeTrabajadores DECIMAL(5,2),
            @PorcentajeIVA DECIMAL(5,2),
            @PorcentajeCruzRoja DECIMAL(5,2),
            @Porcentaje911 DECIMAL(5,2),
            @MontoTotalBruto DECIMAL(10,2),
            @MontoTotalConRebajas DECIMAL(10,2),
            @MontoDomusNet DECIMAL(10,2),
            @MontoTrabajadores DECIMAL(10,2),
            @IdIngresoMensual INT,
            @PorcentajeRebajas DECIMAL(5,2),
            @FechaInicio DATE,
            @FechaFin DATE;

        SET @Quincena = NULLIF(LTRIM(RTRIM(@Quincena)), '');

        IF @Mes < 1 OR @Mes > 12
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1 AS Resultado, 'El mes no es válido.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        SET @FechaInicio = DATEFROMPARTS(@Anio, @Mes, 1);
        SET @FechaFin = DATEADD(MONTH, 1, @FechaInicio);

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Usuarios
            WHERE IdUsuario = @IdRegistradoPor
              AND Activo = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -2 AS Resultado, 'El usuario que registra no existe o está inactivo.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        SELECT TOP 1
            @IdConfiguracion = IdConfiguracion,
            @PorcentajeDomusNet = PorcentajeDomusNet,
            @PorcentajeTrabajadores = PorcentajeTrabajadores,
            @PorcentajeIVA = PorcentajeIVA,
            @PorcentajeCruzRoja = PorcentajeCruzRoja,
            @Porcentaje911 = Porcentaje911
        FROM dbo.ConfiguracionDistribucionIngresos
        WHERE Activo = 1
        ORDER BY IdConfiguracion DESC;

        IF @IdConfiguracion IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -3 AS Resultado, 'No existe una configuración de distribución activa.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        SELECT
            @MontoTotalBruto = SUM(Monto)
        FROM dbo.Ingresos
        WHERE LOWER(LTRIM(RTRIM(Estado))) IN ('registrado', 'registrada')
          AND Fecha >= @FechaInicio
          AND Fecha < @FechaFin
          AND (
                @Quincena IS NULL
                OR (
                    LOWER(@Quincena) = 'primera'
                    AND DAY(Fecha) BETWEEN 1 AND 15
                )
                OR (
                    LOWER(@Quincena) = 'segunda'
                    AND DAY(Fecha) >= 16
                )
              );

        IF @MontoTotalBruto IS NULL OR @MontoTotalBruto <= 0
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -4 AS Resultado, 'No hay ingresos registrados para ese periodo.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        IF EXISTS (
            SELECT 1
            FROM dbo.IngresosMensuales
            WHERE Mes = @Mes
              AND Anio = @Anio
              AND ISNULL(Quincena, '') = ISNULL(@Quincena, '')
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -5 AS Resultado, 'Ya existe un reporte generado para ese periodo.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        SET @PorcentajeRebajas = @PorcentajeIVA + @PorcentajeCruzRoja + @Porcentaje911;

        SET @MontoTotalConRebajas =
            @MontoTotalBruto - ((@MontoTotalBruto * @PorcentajeRebajas) / 100);

        SET @MontoDomusNet =
            (@MontoTotalConRebajas * @PorcentajeDomusNet) / 100;

        SET @MontoTrabajadores =
            (@MontoTotalConRebajas * @PorcentajeTrabajadores) / 100;

        INSERT INTO dbo.IngresosMensuales (
            Mes,
            Anio,
            Quincena,
            MontoTotalBruto,
            MontoTotalConRebajas,
            PorcentajeDomusNet,
            MontoDomusNet,
            PorcentajeTrabajadores,
            MontoTrabajadores,
            PorcentajeIVA,
            PorcentajeCruzRoja,
            Porcentaje911,
            IdConfiguracion,
            IdRegistradoPor,
            Notas
        )
        VALUES (
            @Mes,
            @Anio,
            @Quincena,
            @MontoTotalBruto,
            @MontoTotalConRebajas,
            @PorcentajeDomusNet,
            @MontoDomusNet,
            @PorcentajeTrabajadores,
            @MontoTrabajadores,
            @PorcentajeIVA,
            @PorcentajeCruzRoja,
            @Porcentaje911,
            @IdConfiguracion,
            @IdRegistradoPor,
            @Notas
        );

        SET @IdIngresoMensual = SCOPE_IDENTITY();

        INSERT INTO dbo.DistribucionIngresosMensuales (
            IdIngresoMensual,
            IdUsuario,
            PorcentajeAplicado,
            MontoAsignado
        )
        SELECT
            @IdIngresoMensual,
            IdUsuario,
            Porcentaje,
            (@MontoTotalConRebajas * Porcentaje) / 100
        FROM dbo.ConfiguracionDistribucionTrabajadores
        WHERE IdConfiguracion = @IdConfiguracion
          AND Activo = 1;

        COMMIT TRANSACTION;

        SELECT
            1 AS Resultado,
            'Reporte de ingresos generado correctamente.' AS Mensaje,
            @IdIngresoMensual AS IdGenerado;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT
            -99 AS Resultado,
            ERROR_MESSAGE() AS Mensaje,
            0 AS IdGenerado;
    END CATCH
END;
GO



--Registrar ingresos manuales
CREATE OR ALTER PROCEDURE dbo.registrarIngresoManual
    @IdCliente INT = NULL,
    @IdPaquete INT = NULL,
    @Monto DECIMAL(10,2),
    @Fecha DATETIME2 = NULL,
    @Descripcion NVARCHAR(300) = NULL,
    @IdRegistradoPor INT,
    @TipoIngreso NVARCHAR(30),
    @MetodoPago NVARCHAR(30) = NULL,
    @Quincena NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @IdIngreso INT;

        IF @Monto <= 0
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1 AS Resultado, 'El monto debe ser mayor a cero.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        IF NOT EXISTS (
            SELECT 1 
            FROM dbo.Usuarios 
            WHERE IdUsuario = @IdRegistradoPor 
              AND Activo = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -2 AS Resultado, 'El usuario que registra no existe o está inactivo.' AS Mensaje, 0 AS IdGenerado;
            RETURN;
        END

        IF @IdCliente IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1 
                FROM dbo.Clientes 
                WHERE IdCliente = @IdCliente 
                  AND Activo = 1
            )
            BEGIN
                ROLLBACK TRANSACTION;
                SELECT -3 AS Resultado, 'El cliente no existe o no está activo.' AS Mensaje, 0 AS IdGenerado;
                RETURN;
            END
        END

        IF @IdPaquete IS NULL AND @IdCliente IS NOT NULL
        BEGIN
            SELECT TOP 1 @IdPaquete = IdPaquete
            FROM dbo.AsignacionesPaquete
            WHERE IdCliente = @IdCliente
              AND Estado = 'Activa'
            ORDER BY IdAsignacion DESC;
        END

        IF @IdPaquete IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1 
                FROM dbo.PaquetesServicio 
                WHERE IdPaquete = @IdPaquete
            )
            BEGIN
                ROLLBACK TRANSACTION;
                SELECT -4 AS Resultado, 'El paquete no existe.' AS Mensaje, 0 AS IdGenerado;
                RETURN;
            END
        END

        INSERT INTO dbo.Ingresos (
            IdCliente,
            IdPaquete,
            Monto,
            Fecha,
            Descripcion,
            IdRegistradoPor,
            TipoIngreso,
            MetodoPago,
            Quincena,
            Estado
        )
        VALUES (
            @IdCliente,
            @IdPaquete,
            @Monto,
            ISNULL(@Fecha, GETDATE()),
            @Descripcion,
            @IdRegistradoPor,
            @TipoIngreso,
            @MetodoPago,
            @Quincena,
            'Confirmado'
        );

        SET @IdIngreso = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT 
            1 AS Resultado,
            'Ingreso registrado correctamente.' AS Mensaje,
            @IdIngreso AS IdGenerado;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            -99 AS Resultado,
            ERROR_MESSAGE() AS Mensaje,
            0 AS IdGenerado;
    END CATCH
END;
GO


CREATE OR ALTER PROCEDURE dbo.obtenerDetalleReporteIngreso
    @IdIngresoMensual INT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Resumen general
    SELECT
        im.IdIngresoMensual,
        im.Mes,
        im.Anio,
        im.Quincena,
        im.MontoTotalBruto,
        im.MontoTotalConRebajas,
        im.PorcentajeDomusNet,
        im.MontoDomusNet,
        im.PorcentajeTrabajadores,
        im.MontoTrabajadores,
        im.PorcentajeIVA,
        im.PorcentajeCruzRoja,
        im.Porcentaje911,
        im.Notas,
        im.FechaRegistro,
        u.Nombre AS RegistradoPor
    FROM dbo.IngresosMensuales im
    LEFT JOIN dbo.Usuarios u
        ON im.IdRegistradoPor = u.IdUsuario
    WHERE im.IdIngresoMensual = @IdIngresoMensual;

    -- 2. Totales por tipo
    SELECT
        r.NombreRol AS TipoDistribucion,
        SUM(dim.MontoAsignado) AS TotalAsignado
    FROM dbo.DistribucionIngresosMensuales dim
    INNER JOIN dbo.Usuarios u
        ON dim.IdUsuario = u.IdUsuario
    INNER JOIN dbo.Roles r
        ON u.IdRol = r.IdRol
    WHERE dim.IdIngresoMensual = @IdIngresoMensual
    GROUP BY r.NombreRol
    ORDER BY r.NombreRol;

    -- 3. Distribución por trabajador
    SELECT
        dim.IdUsuario,
        u.Nombre AS Trabajador,
        r.NombreRol AS TipoDistribucion,
        dim.PorcentajeAplicado,
        dim.MontoAsignado
    FROM dbo.DistribucionIngresosMensuales dim
    INNER JOIN dbo.Usuarios u
        ON dim.IdUsuario = u.IdUsuario
    INNER JOIN dbo.Roles r
        ON u.IdRol = r.IdRol
    WHERE dim.IdIngresoMensual = @IdIngresoMensual
    ORDER BY r.NombreRol, u.Nombre;
END;
GO
SELECT * FROM dbo.DistribucionIngresosMensuales
SELECT * FROM dbo.ConfiguracionDistribucionTrabajadores
SELECT * FROM dbo.ConfiguracionDistribucionIngresos
SELECT * FROM dbo.IngresosMensuales
