
USE DomusNet;
GO


IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Usuarios') AND name = 'RefreshToken'
)
BEGIN
    ALTER TABLE Usuarios ADD RefreshToken NVARCHAR(255) NULL;
END
GO



CREATE OR ALTER PROCEDURE modificarToken
    @IdUsuario      INT,
    @RefreshToken   NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuarios
    SET RefreshToken = @RefreshToken,
        FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE verificarTokenR
    @IdUsuario      INT,
    @RefreshToken   NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdRol
    FROM Usuarios
    WHERE IdUsuario = @IdUsuario
      AND RefreshToken = @RefreshToken
      AND Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE buscarUsuarioLogin
    @Correo NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Correo, u.Contrasena, u.Telefono,
           u.IdRol, u.Activo, r.NombreRol
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.Correo = @Correo AND u.Activo = 1;
END
GO
