/*
    Script: Ajustes_DomusNet.sql
    Uso recomendado: ejecutar DESPUÉS de Tablas.sql y ANTES de los procedimientos almacenados.

    Corrige diferencias entre la estructura creada en Tablas.sql y las columnas/estados
    que usan ingresos.sql y PA-Generales.sql.

    Es idempotente: se puede ejecutar varias veces sin crear columnas duplicadas.
*/

USE DomusNet;
GO

SET NOCOUNT ON;
GO

/* =========================================================
   1. Columnas faltantes en dbo.Ingresos
   ========================================================= */

IF COL_LENGTH('dbo.Ingresos', 'TipoIngreso') IS NULL
BEGIN
    ALTER TABLE dbo.Ingresos
    ADD TipoIngreso NVARCHAR(30) NULL;
END;
GO

IF COL_LENGTH('dbo.Ingresos', 'MetodoPago') IS NULL
BEGIN
    ALTER TABLE dbo.Ingresos
    ADD MetodoPago NVARCHAR(50) NULL;
END;
GO

IF COL_LENGTH('dbo.Ingresos', 'Quincena') IS NULL
BEGIN
    ALTER TABLE dbo.Ingresos
    ADD Quincena NVARCHAR(20) NULL;
END;
GO

IF COL_LENGTH('dbo.Ingresos', 'Estado') IS NULL
BEGIN
    ALTER TABLE dbo.Ingresos
    ADD Estado NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Ingresos_Estado DEFAULT ('Registrado');
END;
GO

IF COL_LENGTH('dbo.Ingresos', 'ReferenciaPago') IS NULL
BEGIN
    ALTER TABLE dbo.Ingresos
    ADD ReferenciaPago NVARCHAR(100) NULL;
END;
GO

IF COL_LENGTH('dbo.Ingresos', 'FechaProximoPago') IS NULL
BEGIN
    ALTER TABLE dbo.Ingresos
    ADD FechaProximoPago DATETIME2 NULL;
END;
GO

/* Si Estado ya existía pero estaba nullable o sin datos, normalizamos valores nulos. */
IF COL_LENGTH('dbo.Ingresos', 'Estado') IS NOT NULL
BEGIN
    UPDATE dbo.Ingresos
    SET Estado = 'Registrado'
    WHERE Estado IS NULL;
END;
GO

/* Si la columna Estado ya existía pero no tenía DEFAULT, se agrega. */
IF COL_LENGTH('dbo.Ingresos', 'Estado') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c
        ON dc.parent_object_id = c.object_id
       AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Ingresos')
      AND c.name = 'Estado'
)
BEGIN
    ALTER TABLE dbo.Ingresos
    ADD CONSTRAINT DF_Ingresos_Estado DEFAULT ('Registrado') FOR Estado;
END;
GO

/* =========================================================
   2. Ajustar estados permitidos de SolicitudesServicio
      PA-Generales usa: Realizada y Convertida.
   ========================================================= */

IF EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.SolicitudesServicio')
      AND name = 'CK_Solicitudes_Estado'
)
BEGIN
    ALTER TABLE dbo.SolicitudesServicio
    DROP CONSTRAINT CK_Solicitudes_Estado;
END;
GO

ALTER TABLE dbo.SolicitudesServicio
ADD CONSTRAINT CK_Solicitudes_Estado
CHECK (Estado IN (
    'Pendiente',
    'Programada',
    'PendienteActivacion',
    'Atendida',
    'Realizada',
    'Convertida',
    'Cancelada'
));
GO

/* =========================================================
   3. Ajustar estados permitidos de InstalacionesProgramadas
      La tabla usa Realizada, pero un procedimiento consulta Completada.
      Se permiten ambos para evitar choques entre versiones del código.
   ========================================================= */

IF EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.InstalacionesProgramadas')
      AND name = 'CK_Instalaciones_Estado'
)
BEGIN
    ALTER TABLE dbo.InstalacionesProgramadas
    DROP CONSTRAINT CK_Instalaciones_Estado;
END;
GO

ALTER TABLE dbo.InstalacionesProgramadas
ADD CONSTRAINT CK_Instalaciones_Estado
CHECK (Estado IN (
    'Programada',
    'Realizada',
    'Completada',
    'Cancelada',
    'Reprogramada'
));
GO

/* =========================================================
   4. Verificación rápida
   ========================================================= */

PRINT 'Ajustes aplicados correctamente.';

SELECT
    'Ingresos' AS Tabla,
    c.name AS Columna,
    t.name AS TipoDato,
    c.max_length AS Longitud
FROM sys.columns c
INNER JOIN sys.types t
    ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Ingresos')
  AND c.name IN (
      'TipoIngreso',
      'MetodoPago',
      'Quincena',
      'Estado',
      'ReferenciaPago',
      'FechaProximoPago'
  )
ORDER BY c.column_id;
GO
