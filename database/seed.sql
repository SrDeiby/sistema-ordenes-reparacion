/*
  Datos de ejemplo (inventados) para probar el sistema.
  Ejecutar después de schema.sql, sobre una base vacía:
    sqlcmd -S localhost -E -f 65001 -i database/seed.sql

  Credenciales de prueba:
    Administrador: admin / Admin2025!
    Mecánico:      CarlosMoraVargas / Demo2025!
    Cliente:       LauraBrenesArias / Demo2025!
  (Todas las cuentas de mecánicos y clientes usan Demo2025!)
*/

USE SistemaDeOrdenesDeReparacion;
GO

SET NOCOUNT ON;
BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.Mecanicos ON;
INSERT INTO dbo.Mecanicos (Id, Nombre, Apellidos, Identificacion, CorreoElectronico) VALUES
 (1, N'Carlos', N'Mora Vargas', N'501110222', N'carlos.mora@example.com'),
 (2, N'Andrea', N'Castro Ruiz', N'502220333', N'andrea.castro@example.com');
SET IDENTITY_INSERT dbo.Mecanicos OFF;

SET IDENTITY_INSERT dbo.Clientes ON;
INSERT INTO dbo.Clientes (Id, Nombre, Apellidos, Identificacion, CorreoElectronico) VALUES
 (1, N'Laura',      N'Brenes Arias', N'503330444', N'laura.brenes@example.com'),
 (2, N'Jose Pablo', N'Ugalde Soto',  N'504440555', N'jpugalde@example.com'),
 (3, N'Daniela',    N'Rojas Campos', N'505550666', N'daniela.rojas@example.com');
SET IDENTITY_INSERT dbo.Clientes OFF;

/* Contraseñas guardadas con hash BCrypt. */
INSERT INTO dbo.Usuarios (Nombre, Contrasena, Identificacion, Tipo, Estado, IntentosFallidos, FechaBloqueo, CorreoElectronico) VALUES
 (N'admin',               N'$2a$10$fbXqOhCQ.d/UyDhm6CWB6Oggh0bqyrKycQpOp9743iqqsV1WoBD0a',   N'100000001', 3, 1, 0, NULL, N'admin@example.com'),
 (N'CarlosMoraVargas',    N'$2a$10$1xzRTR4UbuXKL6Qhvql0m.ddbZnFnYtQQ/d5CfSSc83Bkl0K7zaxe', N'501110222', 1, 1, 0, NULL, N'carlos.mora@example.com'),
 (N'AndreaCastroRuiz',    N'$2a$10$ZtQ7x.sXcYFxQEw/II3yGuDwxn2bfW7lawClpY29coEFomMfUquWu', N'502220333', 1, 1, 0, NULL, N'andrea.castro@example.com'),
 (N'LauraBrenesArias',    N'$2a$10$iIkukRdIEBl/TTFYM23G6.79CrznwWk8t28uueyrtyKfRZL75/az2', N'503330444', 2, 1, 0, NULL, N'laura.brenes@example.com'),
 (N'JosePabloUgaldeSoto', N'$2a$10$fIVxrANoOdZGsVnxZlpYQ.LfFie3jOWqhGEzx3iEZ4t0tpT1rWntm', N'504440555', 2, 1, 0, NULL, N'jpugalde@example.com'),
 (N'DanielaRojasCampos',  N'$2a$10$Zekhf77rBCOs5n2wNHP5yeWjghsXj19flsMbQ2Z8fyVuIjcWwsTAW', N'505550666', 2, 1, 0, NULL, N'daniela.rojas@example.com');

SET IDENTITY_INSERT dbo.InventarioDeRepuestos ON;
INSERT INTO dbo.InventarioDeRepuestos (Id, Nombre, Descripcion, PrecioUnitario) VALUES
 (1, N'Filtro de aceite',              N'Filtro de aceite estándar para motores de gasolina y diésel', 4500.00),
 (2, N'Aceite de motor 5W-30',         N'Aceite sintético, precio por litro',                          6500.00),
 (3, N'Pastillas de freno delanteras', N'Juego de pastillas cerámicas para eje delantero',             28000.00),
 (4, N'Bujía de iridio',               N'Bujía de larga duración, precio por unidad',                  9500.00),
 (5, N'Batería 12V 60Ah',              N'Batería libre de mantenimiento',                              65000.00),
 (6, N'Filtro de aire',                N'Filtro de aire del motor',                                    8000.00);
SET IDENTITY_INSERT dbo.InventarioDeRepuestos OFF;

SET IDENTITY_INSERT dbo.InventarioDeServicios ON;
INSERT INTO dbo.InventarioDeServicios (Id, Nombre, Descripcion, Precio) VALUES
 (1, N'Cambio de aceite y filtro',       N'Drenado del aceite usado, cambio de filtro y llenado con aceite nuevo', 12000.00),
 (2, N'Revisión y cambio de frenos',     N'Inspección de discos y pastillas, cambio de pastillas si es necesario', 18000.00),
 (3, N'Diagnóstico por escáner',         N'Lectura de códigos de falla de la computadora del vehículo',          15000.00),
 (4, N'Alineación y balanceo',           N'Alineación de dirección y balanceo de las cuatro llantas',            20000.00),
 (5, N'Cambio de batería',               N'Instalación de batería nueva y prueba del sistema de carga',           5000.00),
 (6, N'Revisión del sistema eléctrico',  N'Revisión de luces, alternador y cableado',                            22000.00);
SET IDENTITY_INSERT dbo.InventarioDeServicios OFF;

/* Estado: 1 Registrado, 2 En proceso, 3 Finalizado, 4 Cancelado
   TipoDeVehiculo: 1 Automóvil, 2 SUV, 3 Pick Up
   TipoDeCombustionInterna: 1 Gasolina, 2 Diésel */
SET IDENTITY_INSERT dbo.OrdenesDeTrabajos ON;
INSERT INTO dbo.OrdenesDeTrabajos
 (Id, Id_Cliente, DescripcionDelProblema, FechaDeRegistro, FechaAproximadaDeFinalizacion, Estado, Id_Mecanico,
  TipoDeVehiculo, TipoDeCombustionInterna, Marca, Modelo, [AñoDeFabricacion], MotivoDeCancelacion) VALUES
 (1, 1, N'Ruido metálico al frenar en la rueda delantera izquierda',
     DATEADD(DAY, -1, SYSDATETIME()), DATEADD(DAY, 3, SYSDATETIME()), 1, NULL, 1, 1, N'Toyota',  N'Corolla',  N'2016', NULL),
 (2, 2, N'Mantenimiento de 60 000 km: cambio de aceite y revisión de frenos',
     DATEADD(DAY, -3, SYSDATETIME()), DATEADD(DAY, 2, SYSDATETIME()), 2, 1,    3, 2, N'Nissan',  N'Frontier', N'2019', NULL),
 (3, 3, N'Luz de check engine encendida y el motor tiembla en ralentí',
     DATEADD(DAY, -10, SYSDATETIME()), DATEADD(DAY, -6, SYSDATETIME()), 3, 2,  2, 1, N'Hyundai', N'Tucson',   N'2021', NULL),
 (4, 1, N'Alineación y balanceo antes de un viaje',
     DATEADD(DAY, -7, SYSDATETIME()), DATEADD(DAY, -5, SYSDATETIME()), 4, NULL, 1, 1, N'Toyota',  N'Corolla',  N'2016',
     N'El cliente decidió realizar el trabajo en otro taller');
SET IDENTITY_INSERT dbo.OrdenesDeTrabajos OFF;

INSERT INTO dbo.OrdenesDeTrabajoInventarioDeServicios (Id_OrdenesDeTrabajo, Id_InventarioDeServicios, Cantidad, Total) VALUES
 (2, 1, 1, 12000.00),
 (2, 2, 1, 18000.00),
 (3, 3, 1, 15000.00);

INSERT INTO dbo.OrdenesDeTrabajoInventarioDeRepuestos (Id_OrdenesDeTrabajos, Id_InventarioDeRepuestos, Cantidad, Total) VALUES
 (2, 1, 1,  4500.00),
 (2, 2, 6, 39000.00),
 (2, 3, 1, 28000.00),
 (3, 4, 4, 38000.00);

COMMIT TRANSACTION;
GO
