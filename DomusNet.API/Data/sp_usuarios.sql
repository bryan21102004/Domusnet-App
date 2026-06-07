USE DomusNet;
GO

CREATE OR ALTER PROCEDURE listarUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Correo, u.Telefono, u.IdRol,
           r.NombreRol, u.Activo, u.FechaCreacion
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    ORDER BY u.Nombre;
END
GO

CREATE OR ALTER PROCEDURE buscarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Correo, u.Telefono, u.IdRol,
           r.NombreRol, u.Activo, u.FechaCreacion
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.IdUsuario = @IdUsuario;
END
GO

-- Retorna: 1=ok, -1=correo duplicado, -2=rol invalido
CREATE OR ALTER PROCEDURE nuevoUsuario
    @Nombre         NVARCHAR(100),
    @Correo         NVARCHAR(150),
    @Contrasena     NVARCHAR(255),
    @Telefono       NVARCHAR(20) = NULL,
    @IdRol          INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Roles WHERE IdRol = @IdRol)
    BEGIN
        SELECT -2 AS Resultado, 0 AS IdGenerado;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo)
    BEGIN
        SELECT -1 AS Resultado, 0 AS IdGenerado;
        RETURN;
    END

    INSERT INTO Usuarios (Nombre, Correo, Contrasena, Telefono, IdRol, Activo, FechaCreacion)
    VALUES (@Nombre, @Correo, @Contrasena, @Telefono, @IdRol, 1, GETUTCDATE());

    SELECT 1 AS Resultado, SCOPE_IDENTITY() AS IdGenerado;
END
GO

-- Retorna: 1=ok, 0=no existe, -1=correo duplicado, -2=rol invalido
CREATE OR ALTER PROCEDURE editarUsuario
    @IdUsuario      INT,
    @Nombre         NVARCHAR(100),
    @Correo         NVARCHAR(150),
    @Telefono       NVARCHAR(20) = NULL,
    @IdRol          INT,
    @Activo         BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Roles WHERE IdRol = @IdRol)
    BEGIN
        SELECT -2 AS Resultado;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo AND IdUsuario <> @IdUsuario)
    BEGIN
        SELECT -1 AS Resultado;
        RETURN;
    END

    UPDATE Usuarios
    SET Nombre = @Nombre, Correo = @Correo, Telefono = @Telefono,
        IdRol = @IdRol, Activo = @Activo, FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;

    SELECT 1 AS Resultado;
END
GO

-- Retorna: 1=ok, 0=no existe
CREATE OR ALTER PROCEDURE eliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE Usuarios SET Activo = 0, FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;

    SELECT 1 AS Resultado;
END
GO

-- Retorna: 1=ok, 0=no existe
CREATE OR ALTER PROCEDURE cambiarContrasenaUsuario
    @IdUsuario      INT,
    @Contrasena     NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    UPDATE Usuarios
    SET Contrasena = @Contrasena, FechaModificacion = GETUTCDATE()
    WHERE IdUsuario = @IdUsuario;

    SELECT 1 AS Resultado;
END
GO
