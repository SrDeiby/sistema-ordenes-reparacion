/*
  Sistema de Órdenes de Reparación - esquema de base de datos (SQL Server)
  Crea la base de datos y las tablas que espera DBContexto (EF Core).
  Ejecutar completo en SSMS o con:
    sqlcmd -S localhost -E -f 65001 -i database/schema.sql
*/

IF DB_ID(N'SistemaDeOrdenesDeReparacion') IS NULL
    CREATE DATABASE SistemaDeOrdenesDeReparacion;
GO

USE SistemaDeOrdenesDeReparacion;
GO

/* Tipo: 1 = Mecánico, 2 = Cliente, 3 = Administrador
   Estado: 1 = Activo, 2 = Bloqueado
   Contrasena guarda el hash BCrypt (60 caracteres). */
CREATE TABLE dbo.Usuarios (
    Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
    Nombre             NVARCHAR(100)  NOT NULL,
    Contrasena         NVARCHAR(100)  NOT NULL,
    Identificacion     NVARCHAR(30)   NULL,
    Tipo               INT            NOT NULL,
    Estado             INT            NULL,
    IntentosFallidos   INT            NULL,
    FechaBloqueo       DATETIME2      NULL,
    CorreoElectronico  NVARCHAR(150)  NULL,
    CONSTRAINT UQ_Usuarios_Nombre UNIQUE (Nombre)
);

CREATE TABLE dbo.Clientes (
    Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clientes PRIMARY KEY,
    Nombre             NVARCHAR(100)  NOT NULL,
    Apellidos          NVARCHAR(100)  NOT NULL,
    Identificacion     NVARCHAR(30)   NOT NULL CONSTRAINT UQ_Clientes_Identificacion UNIQUE,
    CorreoElectronico  NVARCHAR(150)  NULL
);

CREATE TABLE dbo.Mecanicos (
    Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Mecanicos PRIMARY KEY,
    Nombre             NVARCHAR(100)  NOT NULL,
    Apellidos          NVARCHAR(100)  NOT NULL,
    Identificacion     NVARCHAR(30)   NOT NULL CONSTRAINT UQ_Mecanicos_Identificacion UNIQUE,
    CorreoElectronico  NVARCHAR(150)  NULL
);

CREATE TABLE dbo.InventarioDeRepuestos (
    Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InventarioDeRepuestos PRIMARY KEY,
    Nombre             NVARCHAR(100)  NOT NULL,
    Descripcion        NVARCHAR(500)  NOT NULL,
    PrecioUnitario     DECIMAL(18,2)  NOT NULL
);

CREATE TABLE dbo.InventarioDeServicios (
    Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InventarioDeServicios PRIMARY KEY,
    Nombre             NVARCHAR(100)  NOT NULL,
    Descripcion        NVARCHAR(500)  NOT NULL,
    Precio             DECIMAL(18,2)  NOT NULL
);

/* Estado: 1 = Registrado, 2 = En proceso, 3 = Finalizado, 4 = Cancelado
   TipoDeVehiculo y TipoDeCombustionInterna: ver los enum del proyecto Model. */
CREATE TABLE dbo.OrdenesDeTrabajos (
    Id                              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrdenesDeTrabajos PRIMARY KEY,
    Id_Cliente                      INT            NOT NULL,
    DescripcionDelProblema          NVARCHAR(1000) NOT NULL,
    FechaDeRegistro                 DATETIME2      NOT NULL,
    FechaAproximadaDeFinalizacion   DATETIME2      NULL,
    Estado                          INT            NOT NULL,
    Id_Mecanico                     INT            NULL,
    TipoDeVehiculo                  INT            NOT NULL,
    TipoDeCombustionInterna         INT            NOT NULL,
    Marca                           NVARCHAR(50)   NOT NULL,
    Modelo                          NVARCHAR(50)   NOT NULL,
    [AñoDeFabricacion]              NVARCHAR(4)    NOT NULL,
    MotivoDeCancelacion             NVARCHAR(500)  NULL,
    CONSTRAINT FK_OrdenesDeTrabajos_Clientes  FOREIGN KEY (Id_Cliente)  REFERENCES dbo.Clientes (Id),
    CONSTRAINT FK_OrdenesDeTrabajos_Mecanicos FOREIGN KEY (Id_Mecanico) REFERENCES dbo.Mecanicos (Id)
);

CREATE TABLE dbo.OrdenesDeTrabajoInventarioDeRepuestos (
    Id                        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrdenesDeTrabajoInventarioDeRepuestos PRIMARY KEY,
    Id_OrdenesDeTrabajos      INT            NOT NULL,
    Id_InventarioDeRepuestos  INT            NOT NULL,
    Cantidad                  INT            NOT NULL,
    Total                     DECIMAL(18,2)  NOT NULL,
    CONSTRAINT FK_OTRepuestos_Orden    FOREIGN KEY (Id_OrdenesDeTrabajos)     REFERENCES dbo.OrdenesDeTrabajos (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OTRepuestos_Repuesto FOREIGN KEY (Id_InventarioDeRepuestos) REFERENCES dbo.InventarioDeRepuestos (Id)
);

CREATE TABLE dbo.OrdenesDeTrabajoInventarioDeServicios (
    Id                        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrdenesDeTrabajoInventarioDeServicios PRIMARY KEY,
    Id_OrdenesDeTrabajo       INT            NOT NULL,
    Id_InventarioDeServicios  INT            NOT NULL,
    Cantidad                  INT            NOT NULL,
    Total                     DECIMAL(18,2)  NOT NULL,
    CONSTRAINT FK_OTServicios_Orden    FOREIGN KEY (Id_OrdenesDeTrabajo)      REFERENCES dbo.OrdenesDeTrabajos (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OTServicios_Servicio FOREIGN KEY (Id_InventarioDeServicios) REFERENCES dbo.InventarioDeServicios (Id)
);
GO
