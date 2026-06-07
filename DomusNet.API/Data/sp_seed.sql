USE DomusNet;
GO

-- Usuario admin inicial para pruebas
-- Correo: admin@domusnet.com
-- Password: Admin123!
-- (hash generado con ASP.NET Identity PasswordHasher)

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'admin@domusnet.com')
BEGIN
    INSERT INTO Usuarios (Nombre, Correo, Contrasena, Telefono, IdRol, Activo, FechaCreacion)
    VALUES (
        'Administrador',
        'admin@domusnet.com',
        'AQAAAAIAAYagAAAAENz5kF8xV2mQ8pL3nR4xK9vJ2wH6yT1cM0bN5dE7fG8hI9jK0l=',
        '88880000',
        1,
        1,
        GETUTCDATE()
    );
    PRINT 'Admin creado: admin@domusnet.com / Admin123!';
END
ELSE
    PRINT 'El admin ya existe.';
GO

-- Paquetes de ejemplo
IF NOT EXISTS (SELECT 1 FROM PaquetesServicio)
BEGIN
    INSERT INTO PaquetesServicio (Nombre, Descripcion, Velocidad, Precio, PorcentajeDistribucion, Estado, FechaCreacion)
    VALUES
        ('Plan Basico 25 Mbps',  'Internet basico',  '25 Mbps',  10000, 10, 'Activo', GETUTCDATE()),
        ('Plan Familiar 50 Mbps','Internet familiar', '50 Mbps',  15000, 10, 'Activo', GETUTCDATE()),
        ('Plan Premium 100 Mbps','Internet premium',  '100 Mbps', 25000, 10, 'Activo', GETUTCDATE());
    PRINT 'Paquetes de ejemplo insertados.';
END
GO
