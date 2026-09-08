SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

GO



IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE TABLE [ProductionLines] (
        [Id] int NOT NULL IDENTITY,
        [DepartmentId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ProductionLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductionLines_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeNumber] varchar(20) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [DepartmentId] int NOT NULL,
        [LineId] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employees_ProductionLines_LineId] FOREIGN KEY ([LineId]) REFERENCES [ProductionLines] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Departments_Name] ON [Departments] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE INDEX [IX_Employees_DepartmentId] ON [Employees] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_EmployeeNumber] ON [Employees] ([EmployeeNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE INDEX [IX_Employees_LineId] ON [Employees] ([LineId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProductionLines_DepartmentId_Name] ON [ProductionLines] ([DepartmentId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811225043_InitialOrganization'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260811225043_InitialOrganization', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL IDENTITY,
        [Name] varchar(50) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] int NOT NULL,
        [Username] varchar(100) NOT NULL,
        [PasswordHash] varchar(255) NOT NULL,
        [IsActive] bit NOT NULL,
        [LastLoginAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [Description], [IsActive], [Name])
    VALUES (1, N''Full system administration.'', CAST(1 AS bit), ''Administrator''),
    (2, N''Production operations.'', CAST(1 AS bit), ''Production''),
    (3, N''Warehouse operations.'', CAST(1 AS bit), ''Warehouse''),
    (4, N''Read-only access.'', CAST(1 AS bit), ''Viewer'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_EmployeeId] ON [Users] ([EmployeeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812161517_AddSecurity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260812161517_AddSecurity', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE TABLE [PPECategories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] int NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] int NULL,
        CONSTRAINT [PK_PPECategories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PPECategories_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPECategories_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE TABLE [PPEProducts] (
        [Id] int NOT NULL IDENTITY,
        [SKU] AS 'EPP-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6) PERSISTED,
        [CategoryId] int NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Size] nvarchar(50) NULL,
        [Color] nvarchar(50) NULL,
        [Model] nvarchar(100) NULL,
        [Specification] nvarchar(250) NULL,
        [StockUnit] nvarchar(30) NOT NULL,
        [MinimumStock] int NOT NULL,
        [MaxQuantityPerRequest] int NULL,
        [ReplacementIntervalDays] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] int NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] int NULL,
        CONSTRAINT [PK_PPEProducts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PPEProducts_PPECategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [PPECategories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPEProducts_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPEProducts_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE INDEX [IX_PPECategories_CreatedByUserId] ON [PPECategories] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PPECategories_Name] ON [PPECategories] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE INDEX [IX_PPECategories_UpdatedByUserId] ON [PPECategories] ([UpdatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE INDEX [IX_PPEProducts_CategoryId] ON [PPEProducts] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE INDEX [IX_PPEProducts_CreatedByUserId] ON [PPEProducts] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PPEProducts_SKU] ON [PPEProducts] ([SKU]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    CREATE INDEX [IX_PPEProducts_UpdatedByUserId] ON [PPEProducts] ([UpdatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812204114_AddPPECatalog'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260812204114_AddPPECatalog', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE TABLE [Suppliers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [ContactName] nvarchar(150) NULL,
        [Email] varchar(200) NULL,
        [Phone] varchar(30) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] int NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] int NULL,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Suppliers_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Suppliers_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE TABLE [Warehouses] (
        [Id] int NOT NULL IDENTITY,
        [Code] varchar(20) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] int NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] int NULL,
        CONSTRAINT [PK_Warehouses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Warehouses_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Warehouses_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE TABLE [ProductSuppliers] (
        [PPEProductId] int NOT NULL,
        [SupplierId] int NOT NULL,
        [SupplierProductCode] varchar(100) NULL,
        [PackageBarcode] varchar(100) NULL,
        [PurchaseUnit] nvarchar(30) NOT NULL,
        [UnitsPerPackage] int NOT NULL,
        [IsPreferred] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] int NOT NULL,
        CONSTRAINT [PK_ProductSuppliers] PRIMARY KEY ([PPEProductId], [SupplierId]),
        CONSTRAINT [FK_ProductSuppliers_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProductSuppliers_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProductSuppliers_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE INDEX [IX_ProductSuppliers_CreatedByUserId] ON [ProductSuppliers] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ProductSuppliers_PPEProductId] ON [ProductSuppliers] ([PPEProductId]) WHERE [IsPreferred] = 1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE INDEX [IX_ProductSuppliers_SupplierId] ON [ProductSuppliers] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE INDEX [IX_Suppliers_CreatedByUserId] ON [Suppliers] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Suppliers_Name] ON [Suppliers] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE INDEX [IX_Suppliers_UpdatedByUserId] ON [Suppliers] ([UpdatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Warehouses_Code] ON [Warehouses] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE INDEX [IX_Warehouses_CreatedByUserId] ON [Warehouses] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Warehouses_Name] ON [Warehouses] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    CREATE INDEX [IX_Warehouses_UpdatedByUserId] ON [Warehouses] ([UpdatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260812224730_AddSupplyCatalog'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260812224730_AddSupplyCatalog', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE TABLE [PurchaseOrders] (
        [Id] int NOT NULL IDENTITY,
        [Folio] AS 'PO-' + CONVERT(varchar(4), DATEPART(year, [OrderDate])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6) PERSISTED,
        [SupplierId] int NOT NULL,
        [PurchaseOrderNumber] varchar(50) NOT NULL,
        [Status] varchar(20) NOT NULL,
        [OrderDate] date NOT NULL,
        [ConfirmedDeliveryDate] date NOT NULL,
        [SupplierConfirmedAt] datetime2 NULL,
        [CurrencyCode] varchar(3) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] int NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] int NULL,
        [CancelledAt] datetime2 NULL,
        [CancelledByUserId] int NULL,
        [CancellationReason] nvarchar(500) NULL,
        CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseOrders_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseOrders_Users_CancelledByUserId] FOREIGN KEY ([CancelledByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseOrders_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseOrders_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE TABLE [PurchaseOrderItems] (
        [Id] int NOT NULL IDENTITY,
        [PurchaseOrderId] int NOT NULL,
        [PPEProductId] int NOT NULL,
        [SupplierProductCode] varchar(100) NULL,
        [PurchaseUnit] nvarchar(30) NOT NULL,
        [UnitsPerPackage] int NOT NULL,
        [OrderedPurchaseQuantity] int NOT NULL,
        [PurchaseUnitCost] decimal(18,4) NULL,
        CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseOrderItems_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrderItems_PPEProductId] ON [PurchaseOrderItems] ([PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PurchaseOrderItems_PurchaseOrderId_PPEProductId] ON [PurchaseOrderItems] ([PurchaseOrderId], [PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrders_CancelledByUserId] ON [PurchaseOrders] ([CancelledByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrders_CreatedByUserId] ON [PurchaseOrders] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PurchaseOrders_Folio] ON [PurchaseOrders] ([Folio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PurchaseOrders_SupplierId_PurchaseOrderNumber] ON [PurchaseOrders] ([SupplierId], [PurchaseOrderNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrders_UpdatedByUserId] ON [PurchaseOrders] ([UpdatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813134130_AddPurchaseOrders'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813134130_AddPurchaseOrders', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE TABLE [GoodsReceipts] (
        [Id] int NOT NULL IDENTITY,
        [Folio] AS 'GR-' + CONVERT(varchar(4), DATEPART(year, [ReceivedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6) PERSISTED,
        [PurchaseOrderId] int NOT NULL,
        [WarehouseId] int NOT NULL,
        [ReceivedAt] datetime2 NOT NULL,
        [ReceivedByUserId] int NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_GoodsReceipts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GoodsReceipts_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GoodsReceipts_Users_ReceivedByUserId] FOREIGN KEY ([ReceivedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GoodsReceipts_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE TABLE [InventoryBalances] (
        [WarehouseId] int NOT NULL,
        [PPEProductId] int NOT NULL,
        [OnHandQuantity] int NOT NULL,
        [ReservedQuantity] int NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_InventoryBalances] PRIMARY KEY ([WarehouseId], [PPEProductId]),
        CONSTRAINT [CK_InventoryBalances_OnHand] CHECK ([OnHandQuantity] >= 0),
        CONSTRAINT [CK_InventoryBalances_Reserved] CHECK ([ReservedQuantity] >= 0),
        CONSTRAINT [CK_InventoryBalances_Reserved_OnHand] CHECK ([ReservedQuantity] <= [OnHandQuantity]),
        CONSTRAINT [FK_InventoryBalances_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryBalances_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE TABLE [InventoryMovements] (
        [Id] bigint NOT NULL IDENTITY,
        [WarehouseId] int NOT NULL,
        [PPEProductId] int NOT NULL,
        [MovementType] varchar(30) NOT NULL,
        [Quantity] int NOT NULL,
        [ReferenceType] varchar(30) NOT NULL,
        [ReferenceId] int NOT NULL,
        [UnitCost] decimal(18,4) NULL,
        [Reason] nvarchar(500) NULL,
        [CreatedByUserId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_InventoryMovements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InventoryMovements_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryMovements_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryMovements_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE TABLE [GoodsReceiptItems] (
        [Id] int NOT NULL IDENTITY,
        [GoodsReceiptId] int NOT NULL,
        [PurchaseOrderItemId] int NOT NULL,
        [PPEProductId] int NOT NULL,
        [ReceivedQuantity] int NOT NULL,
        CONSTRAINT [PK_GoodsReceiptItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GoodsReceiptItems_GoodsReceipts_GoodsReceiptId] FOREIGN KEY ([GoodsReceiptId]) REFERENCES [GoodsReceipts] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_GoodsReceiptItems_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GoodsReceiptItems_PurchaseOrderItems_PurchaseOrderItemId] FOREIGN KEY ([PurchaseOrderItemId]) REFERENCES [PurchaseOrderItems] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GoodsReceiptItems_GoodsReceiptId_PPEProductId] ON [GoodsReceiptItems] ([GoodsReceiptId], [PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE INDEX [IX_GoodsReceiptItems_PPEProductId] ON [GoodsReceiptItems] ([PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GoodsReceiptItems_PurchaseOrderItemId] ON [GoodsReceiptItems] ([PurchaseOrderItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GoodsReceipts_Folio] ON [GoodsReceipts] ([Folio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GoodsReceipts_PurchaseOrderId] ON [GoodsReceipts] ([PurchaseOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE INDEX [IX_GoodsReceipts_ReceivedByUserId] ON [GoodsReceipts] ([ReceivedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE INDEX [IX_GoodsReceipts_WarehouseId] ON [GoodsReceipts] ([WarehouseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE INDEX [IX_InventoryBalances_PPEProductId] ON [InventoryBalances] ([PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE INDEX [IX_InventoryMovements_CreatedByUserId] ON [InventoryMovements] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE INDEX [IX_InventoryMovements_PPEProductId] ON [InventoryMovements] ([PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    CREATE INDEX [IX_InventoryMovements_WarehouseId_PPEProductId_CreatedAt] ON [InventoryMovements] ([WarehouseId], [PPEProductId], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813153803_AddInventoryReceiving'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813153803_AddInventoryReceiving', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE TABLE [RequestReasons] (
        [Id] int NOT NULL IDENTITY,
        [Code] varchar(50) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_RequestReasons] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE TABLE [PPERequests] (
        [Id] int NOT NULL IDENTITY,
        [Folio] AS 'EPP-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6) PERSISTED,
        [EmployeeId] int NOT NULL,
        [WarehouseId] int NOT NULL,
        [RequestReasonId] int NOT NULL,
        [Status] varchar(20) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedByUserId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [DeliveredByUserId] int NULL,
        [DeliveredAt] datetime2 NULL,
        [CancelledByUserId] int NULL,
        [CancelledAt] datetime2 NULL,
        [CancellationReason] nvarchar(500) NULL,
        CONSTRAINT [PK_PPERequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PPERequests_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPERequests_RequestReasons_RequestReasonId] FOREIGN KEY ([RequestReasonId]) REFERENCES [RequestReasons] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPERequests_Users_CancelledByUserId] FOREIGN KEY ([CancelledByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPERequests_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPERequests_Users_DeliveredByUserId] FOREIGN KEY ([DeliveredByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPERequests_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE TABLE [PPERequestItems] (
        [Id] int NOT NULL IDENTITY,
        [PPERequestId] int NOT NULL,
        [PPEProductId] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_PPERequestItems] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PPERequestItems_Quantity] CHECK ([Quantity] > 0),
        CONSTRAINT [FK_PPERequestItems_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PPERequestItems_PPERequests_PPERequestId] FOREIGN KEY ([PPERequestId]) REFERENCES [PPERequests] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[RequestReasons]'))
        SET IDENTITY_INSERT [RequestReasons] ON;
    EXEC(N'INSERT INTO [RequestReasons] ([Id], [Code], [Description], [IsActive], [Name])
    VALUES (1, ''INITIAL_ASSIGNMENT'', N''Initial PPE assignment.'', CAST(1 AS bit), N''Initial Assignment''),
    (2, ''SCHEDULED_REPLACEMENT'', N''Replacement according to scheduled useful life.'', CAST(1 AS bit), N''Scheduled Replacement''),
    (3, ''WEAR'', N''Replacement due to normal wear.'', CAST(1 AS bit), N''Wear''),
    (4, ''DAMAGE'', N''Replacement due to damage.'', CAST(1 AS bit), N''Damage''),
    (5, ''LOST'', N''Replacement because PPE was lost.'', CAST(1 AS bit), N''Lost''),
    (6, ''JOB_CHANGE'', N''PPE required because of job or position change.'', CAST(1 AS bit), N''Job Change''),
    (7, ''OTHER'', N''Other justified reason.'', CAST(1 AS bit), N''Other'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[RequestReasons]'))
        SET IDENTITY_INSERT [RequestReasons] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE INDEX [IX_PPERequestItems_PPEProductId] ON [PPERequestItems] ([PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PPERequestItems_PPERequestId_PPEProductId] ON [PPERequestItems] ([PPERequestId], [PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE INDEX [IX_PPERequests_CancelledByUserId] ON [PPERequests] ([CancelledByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE INDEX [IX_PPERequests_CreatedByUserId] ON [PPERequests] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE INDEX [IX_PPERequests_DeliveredByUserId] ON [PPERequests] ([DeliveredByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE INDEX [IX_PPERequests_EmployeeId] ON [PPERequests] ([EmployeeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PPERequests_Folio] ON [PPERequests] ([Folio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE INDEX [IX_PPERequests_RequestReasonId] ON [PPERequests] ([RequestReasonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE INDEX [IX_PPERequests_WarehouseId] ON [PPERequests] ([WarehouseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RequestReasons_Code] ON [RequestReasons] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813164425_AddPPERequests'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813164425_AddPPERequests', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE TABLE [InventoryCounts] (
        [Id] int NOT NULL IDENTITY,
        [Folio] AS 'IC-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6) PERSISTED,
        [WarehouseId] int NOT NULL,
        [Status] varchar(20) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedByUserId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [SubmittedByUserId] int NULL,
        [SubmittedAt] datetime2 NULL,
        [PostedByUserId] int NULL,
        [PostedAt] datetime2 NULL,
        [CancelledByUserId] int NULL,
        [CancelledAt] datetime2 NULL,
        [CancellationReason] nvarchar(500) NULL,
        CONSTRAINT [PK_InventoryCounts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InventoryCounts_Users_CancelledByUserId] FOREIGN KEY ([CancelledByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryCounts_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryCounts_Users_PostedByUserId] FOREIGN KEY ([PostedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryCounts_Users_SubmittedByUserId] FOREIGN KEY ([SubmittedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryCounts_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE TABLE [InventoryCountItems] (
        [Id] int NOT NULL IDENTITY,
        [InventoryCountId] int NOT NULL,
        [PPEProductId] int NOT NULL,
        [CountedQuantity] int NULL,
        [SystemQuantitySnapshot] int NULL,
        [Variance] int NULL,
        [CountedByUserId] int NULL,
        [CountedAt] datetime2 NULL,
        CONSTRAINT [PK_InventoryCountItems] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_InventoryCountItems_CountedQuantity] CHECK ([CountedQuantity] IS NULL OR [CountedQuantity] >= 0),
        CONSTRAINT [CK_InventoryCountItems_SystemQuantity] CHECK ([SystemQuantitySnapshot] IS NULL OR [SystemQuantitySnapshot] >= 0),
        CONSTRAINT [FK_InventoryCountItems_InventoryCounts_InventoryCountId] FOREIGN KEY ([InventoryCountId]) REFERENCES [InventoryCounts] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InventoryCountItems_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryCountItems_Users_CountedByUserId] FOREIGN KEY ([CountedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE INDEX [IX_InventoryCountItems_CountedByUserId] ON [InventoryCountItems] ([CountedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryCountItems_InventoryCountId_PPEProductId] ON [InventoryCountItems] ([InventoryCountId], [PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE INDEX [IX_InventoryCountItems_PPEProductId] ON [InventoryCountItems] ([PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE INDEX [IX_InventoryCounts_CancelledByUserId] ON [InventoryCounts] ([CancelledByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE INDEX [IX_InventoryCounts_CreatedByUserId] ON [InventoryCounts] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryCounts_Folio] ON [InventoryCounts] ([Folio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE INDEX [IX_InventoryCounts_PostedByUserId] ON [InventoryCounts] ([PostedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    CREATE INDEX [IX_InventoryCounts_SubmittedByUserId] ON [InventoryCounts] ([SubmittedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_InventoryCounts_WarehouseId] ON [InventoryCounts] ([WarehouseId]) WHERE [Status] IN (''Draft'', ''PendingReview'')');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813210657_AddInventoryCounts'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813210657_AddInventoryCounts', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [EntityName] varchar(100) NOT NULL,
        [EntityId] varchar(100) NOT NULL,
        [Action] varchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [OldValuesJson] nvarchar(max) NULL,
        [NewValuesJson] nvarchar(max) NULL,
        [PerformedByUserId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditLogs_Users_PerformedByUserId] FOREIGN KEY ([PerformedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE TABLE [InventoryAdjustments] (
        [Id] int NOT NULL IDENTITY,
        [Folio] AS 'ADJ-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6) PERSISTED,
        [WarehouseId] int NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [CreatedByUserId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_InventoryAdjustments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InventoryAdjustments_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryAdjustments_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE TABLE [InventoryAdjustmentItems] (
        [Id] int NOT NULL IDENTITY,
        [InventoryAdjustmentId] int NOT NULL,
        [PPEProductId] int NOT NULL,
        [QuantityAdjustment] int NOT NULL,
        [PreviousOnHandQuantity] int NOT NULL,
        [NewOnHandQuantity] int NOT NULL,
        CONSTRAINT [PK_InventoryAdjustmentItems] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_InventoryAdjustmentItems_NewOnHand] CHECK ([NewOnHandQuantity] >= 0),
        CONSTRAINT [CK_InventoryAdjustmentItems_PreviousOnHand] CHECK ([PreviousOnHandQuantity] >= 0),
        CONSTRAINT [CK_InventoryAdjustmentItems_QuantityAdjustment] CHECK ([QuantityAdjustment] <> 0),
        CONSTRAINT [FK_InventoryAdjustmentItems_InventoryAdjustments_InventoryAdjustmentId] FOREIGN KEY ([InventoryAdjustmentId]) REFERENCES [InventoryAdjustments] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InventoryAdjustmentItems_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityName_EntityId] ON [AuditLogs] ([EntityName], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_PerformedByUserId_CreatedAt] ON [AuditLogs] ([PerformedByUserId], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryAdjustmentItems_InventoryAdjustmentId_PPEProductId] ON [InventoryAdjustmentItems] ([InventoryAdjustmentId], [PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE INDEX [IX_InventoryAdjustmentItems_PPEProductId] ON [InventoryAdjustmentItems] ([PPEProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE INDEX [IX_InventoryAdjustments_CreatedByUserId] ON [InventoryAdjustments] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryAdjustments_Folio] ON [InventoryAdjustments] ([Folio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    CREATE INDEX [IX_InventoryAdjustments_WarehouseId] ON [InventoryAdjustments] ([WarehouseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813230236_AddInventoryAdjustmentsAndAuditLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813230236_AddInventoryAdjustmentsAndAuditLogs', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813234159_AddReservedSnapshotToInventoryAdjustment'
)
BEGIN
    ALTER TABLE [InventoryAdjustmentItems] ADD [ReservedQuantitySnapshot] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813234159_AddReservedSnapshotToInventoryAdjustment'
)
BEGIN
    EXEC(N'ALTER TABLE [InventoryAdjustmentItems] ADD CONSTRAINT [CK_InventoryAdjustmentItems_Reserved] CHECK ([ReservedQuantitySnapshot] >= 0)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813234159_AddReservedSnapshotToInventoryAdjustment'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813234159_AddReservedSnapshotToInventoryAdjustment', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817204419_FixPreferredProductSupplierIndex'
)
BEGIN
    DROP INDEX [IX_ProductSuppliers_PPEProductId] ON [ProductSuppliers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817204419_FixPreferredProductSupplierIndex'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ProductSuppliers_PPEProductId] ON [ProductSuppliers] ([PPEProductId]) WHERE [IsPreferred] = 1 AND [IsActive] = 1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260817204419_FixPreferredProductSupplierIndex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260817204419_FixPreferredProductSupplierIndex', N'10.0.11');
END;

COMMIT;
GO

