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
