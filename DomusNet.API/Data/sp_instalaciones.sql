USE DomusNet;
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
        IdInstalacion,
        IdSolicitud,
        IdTecnicoAsignado,
        IdVendedorPrograma,
        FechaProgramada,
        FechaRealizacion,
        UbicacionInstalacion,
        FotoEvidencia,
        PruebaVelocidad,
        Estado,
        NotasVendedor,
        ComentarioTecnico,
        FechaCreacion,
        FechaActualizacion
    FROM InstalacionesProgramadas
    WHERE IdTecnicoAsignado = @IdTecnico
      AND Estado = 'Programada'
    ORDER BY FechaProgramada ASC;
END
GO

-- ────────────────────────────────────────────────────
--  COMPLETAR INSTALACIÓN Y ACTIVAR SERVICIO/CLIENTE
-- ────────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE completarInstalacion
    @IdInstalacion      INT,
    @FotoEvidencia      VARCHAR(MAX),
    @PruebaVelocidad    VARCHAR(500),
    @ComentarioTecnico  VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Verificar si la instalación existe y está en estado 'Programada'
        IF NOT EXISTS (
            SELECT 1 
            FROM InstalacionesProgramadas 
            WHERE IdInstalacion = @IdInstalacion AND Estado = 'Programada'
        )
        BEGIN
            SELECT -1 AS Resultado, 'La instalación no existe o ya no está en estado Programada.' AS Mensaje, NULL AS IdInstalacion;
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DECLARE @IdSolicitud INT;
        SELECT @IdSolicitud = IdSolicitud FROM InstalacionesProgramadas WHERE IdInstalacion = @IdInstalacion;

        -- 2. Actualizar la instalación programada
        UPDATE InstalacionesProgramadas
        SET 
            Estado = 'Completada',
            FechaRealizacion = GETDATE(),
            FotoEvidencia = @FotoEvidencia,
            PruebaVelocidad = @PruebaVelocidad,
            ComentarioTecnico = @ComentarioTecnico,
            FechaActualizacion = GETDATE()
        WHERE IdInstalacion = @IdInstalacion;

        -- 3. Actualizar la Solicitud de Servicio a 'Atendida'
        UPDATE SolicitudesServicio
        SET Estado = 'Atendida'
        WHERE IdSolicitud = @IdSolicitud;

        -- 4. Activar al Cliente asociado (cambiar EstadoPago a 'AlDia' si corresponde)
        DECLARE @IdCliente INT;
        SELECT @IdCliente = IdCliente FROM Clientes WHERE IdSolicitudOrigen = @IdSolicitud AND Activo = 1;

        IF @IdCliente IS NOT NULL
        BEGIN
            UPDATE Clientes
            SET EstadoPago = 'AlDia'
            WHERE IdCliente = @IdCliente;
        END

        COMMIT TRANSACTION;

        SELECT 1 AS Resultado, 'Instalación completada y servicio activado correctamente.' AS Mensaje, @IdInstalacion AS IdInstalacion;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT -2 AS Resultado, ERROR_MESSAGE() AS Mensaje, NULL AS IdInstalacion;
    END CATCH
END
GO
