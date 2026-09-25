BEGIN TRANSACTION;
ALTER TABLE [PPERequests] ADD [OrganizationalUnitId] int NULL;

ALTER TABLE [PPERequestItems] ADD [AppliedMaxQuantityPerRequest] int NULL;

ALTER TABLE [Employees] ADD [OrganizationalUnitId] int NULL;

CREATE TABLE [OrganizationalUnits] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Type] varchar(30) NOT NULL,
    [ParentId] int NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_OrganizationalUnits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrganizationalUnits_OrganizationalUnits_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [OrganizationalUnits] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [OrganizationalUnitPPELimits] (
    [Id] int NOT NULL IDENTITY,
    [OrganizationalUnitId] int NOT NULL,
    [PPEProductId] int NOT NULL,
    [MaxQuantityPerRequest] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_OrganizationalUnitPPELimits] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_OrganizationalUnitPPELimits_MaxQuantity] CHECK ([MaxQuantityPerRequest] > 0),
    CONSTRAINT [FK_OrganizationalUnitPPELimits_OrganizationalUnits_OrganizationalUnitId] FOREIGN KEY ([OrganizationalUnitId]) REFERENCES [OrganizationalUnits] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrganizationalUnitPPELimits_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION
);

/* =========================================================
   1. Departments -> OrganizationalUnits
   ========================================================= */
INSERT INTO OrganizationalUnits
(
    Name,
    Description,
    Type,
    ParentId,
    IsActive,
    CreatedAt,
    UpdatedAt
)
SELECT
    d.Name,
    d.Description,
    'Department',
    NULL,
    d.IsActive,
    d.CreatedAt,
    d.UpdatedAt
FROM Departments d
WHERE NOT EXISTS
(
    SELECT 1
    FROM OrganizationalUnits ou
    WHERE ou.ParentId IS NULL
      AND ou.Type = 'Department'
      AND ou.Name = d.Name
);


/* =========================================================
   2. ProductionLines -> OrganizationalUnits
   ========================================================= */
INSERT INTO OrganizationalUnits
(
    Name,
    Description,
    Type,
    ParentId,
    IsActive,
    CreatedAt,
    UpdatedAt
)
SELECT
    pl.Name,
    pl.Description,
    'Line',
    departmentUnit.Id,
    pl.IsActive,
    pl.CreatedAt,
    pl.UpdatedAt
FROM ProductionLines pl
INNER JOIN Departments d
    ON d.Id = pl.DepartmentId
INNER JOIN OrganizationalUnits departmentUnit
    ON departmentUnit.ParentId IS NULL
   AND departmentUnit.Type = 'Department'
   AND departmentUnit.Name = d.Name
WHERE NOT EXISTS
(
    SELECT 1
    FROM OrganizationalUnits lineUnit
    WHERE lineUnit.ParentId = departmentUnit.Id
      AND lineUnit.Type = 'Line'
      AND lineUnit.Name = pl.Name
);


/* =========================================================
   3. Employees -> OrganizationalUnitId
   Si tiene línea, usa Línea.
   Si no tiene línea, usa Departamento.
   ========================================================= */
UPDATE e
SET OrganizationalUnitId =
    COALESCE(
        lineUnit.Id,
        departmentUnit.Id
    )
FROM Employees e
INNER JOIN Departments d
    ON d.Id = e.DepartmentId
INNER JOIN OrganizationalUnits departmentUnit
    ON departmentUnit.ParentId IS NULL
   AND departmentUnit.Type = 'Department'
   AND departmentUnit.Name = d.Name
LEFT JOIN ProductionLines pl
    ON pl.Id = e.LineId
LEFT JOIN OrganizationalUnits lineUnit
    ON lineUnit.ParentId = departmentUnit.Id
   AND lineUnit.Type = 'Line'
   AND lineUnit.Name = pl.Name
WHERE e.OrganizationalUnitId IS NULL;


/* =========================================================
   4. Snapshot organizacional para solicitudes existentes
   ========================================================= */
UPDATE r
SET OrganizationalUnitId =
    e.OrganizationalUnitId
FROM PPERequests r
INNER JOIN Employees e
    ON e.Id = r.EmployeeId
WHERE r.OrganizationalUnitId IS NULL;


/* =========================================================
   5. Snapshot del límite utilizado en solicitudes anteriores.

   Antes de este refactor, el único límite existente era
   PPEProducts.MaxQuantityPerRequest, así que ese es el
   máximo históricamente aplicable.
   ========================================================= */
UPDATE item
SET AppliedMaxQuantityPerRequest =
    product.MaxQuantityPerRequest
FROM PPERequestItems item
INNER JOIN PPEProducts product
    ON product.Id = item.PPEProductId
WHERE item.AppliedMaxQuantityPerRequest IS NULL;

CREATE INDEX [IX_PPERequests_OrganizationalUnitId] ON [PPERequests] ([OrganizationalUnitId]);

CREATE INDEX [IX_Employees_OrganizationalUnitId] ON [Employees] ([OrganizationalUnitId]);

CREATE UNIQUE INDEX [IX_OrganizationalUnitPPELimits_OrganizationalUnitId_PPEProductId] ON [OrganizationalUnitPPELimits] ([OrganizationalUnitId], [PPEProductId]);

CREATE INDEX [IX_OrganizationalUnitPPELimits_PPEProductId] ON [OrganizationalUnitPPELimits] ([PPEProductId]);

CREATE UNIQUE INDEX [IX_OrganizationalUnits_ParentId_Name] ON [OrganizationalUnits] ([ParentId], [Name]) WHERE [ParentId] IS NOT NULL;

ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_OrganizationalUnits_OrganizationalUnitId] FOREIGN KEY ([OrganizationalUnitId]) REFERENCES [OrganizationalUnits] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [PPERequests] ADD CONSTRAINT [FK_PPERequests_OrganizationalUnits_OrganizationalUnitId] FOREIGN KEY ([OrganizationalUnitId]) REFERENCES [OrganizationalUnits] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260828002456_AddOrganizationalStructure', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'DepartmentId');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Employees] ALTER COLUMN [DepartmentId] int NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260831125343_MakeLegacyEmployeeOrganizationOptional', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [PPERequests] DROP CONSTRAINT [FK_PPERequests_OrganizationalUnits_OrganizationalUnitId];

ALTER TABLE [OrganizationalUnitPPELimits] DROP CONSTRAINT [CK_OrganizationalUnitPPELimits_MaxQuantity];

EXEC sp_rename N'[PPERequests].[OrganizationalUnitId]', N'RequestedForOrganizationalUnitId', 'COLUMN';

EXEC sp_rename N'[PPERequests].[IX_PPERequests_OrganizationalUnitId]', N'IX_PPERequests_RequestedForOrganizationalUnitId', 'INDEX';

EXEC sp_rename N'[PPERequestItems].[AppliedMaxQuantityPerRequest]', N'AppliedMaxQuantityPerCycle', 'COLUMN';

EXEC sp_rename N'[PPEProducts].[MaxQuantityPerRequest]', N'DefaultMaxQuantityPerCycle', 'COLUMN';

EXEC sp_rename N'[OrganizationalUnitPPELimits].[MaxQuantityPerRequest]', N'MaxQuantityPerCycle', 'COLUMN';

ALTER TABLE [OrganizationalUnitPPELimits] ADD CONSTRAINT [CK_OrganizationalUnitPPELimits_MaxQuantity] CHECK ([MaxQuantityPerCycle] > 0);

ALTER TABLE [PPERequests] ADD CONSTRAINT [FK_PPERequests_OrganizationalUnits_RequestedForOrganizationalUnitId] FOREIGN KEY ([RequestedForOrganizationalUnitId]) REFERENCES [OrganizationalUnits] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260908123033_RenamePPECycleLimitFields', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Units] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Symbol] nvarchar(20) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedByUserId] int NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedByUserId] int NULL,
    CONSTRAINT [PK_Units] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Units_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Units_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Units_CreatedByUserId] ON [Units] ([CreatedByUserId]);

CREATE UNIQUE INDEX [IX_Units_Name] ON [Units] ([Name]);

CREATE INDEX [IX_Units_UpdatedByUserId] ON [Units] ([UpdatedByUserId]);

INSERT INTO [Units]
(
    [Name],
    [Symbol],
    [IsActive],
    [CreatedAt],
    [CreatedByUserId],
    [UpdatedAt],
    [UpdatedByUserId]
)
SELECT
    N'Pieza',
    NULL,
    1,
    SYSUTCDATETIME(),
    NULL,
    NULL,
    NULL
WHERE NOT EXISTS
(
    SELECT 1
    FROM [Units]
    WHERE [Name] = N'Pieza'
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260910212903_AddUnitsCatalog', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [ProductSuppliers] ADD [PurchaseUnitId] int NULL;

ALTER TABLE [PPEProducts] ADD [StockUnitId] int NULL;

UPDATE P
SET P.StockUnitId = U.Id
FROM PPEProducts P
INNER JOIN Units U
    ON U.Name = LTRIM(RTRIM(P.StockUnit));

UPDATE PS
SET PS.PurchaseUnitId = U.Id
FROM ProductSuppliers PS
INNER JOIN Units U
    ON U.Name = LTRIM(RTRIM(PS.PurchaseUnit));

CREATE INDEX [IX_ProductSuppliers_PurchaseUnitId] ON [ProductSuppliers] ([PurchaseUnitId]);

CREATE INDEX [IX_PPEProducts_StockUnitId] ON [PPEProducts] ([StockUnitId]);

ALTER TABLE [PPEProducts] ADD CONSTRAINT [FK_PPEProducts_Units_StockUnitId] FOREIGN KEY ([StockUnitId]) REFERENCES [Units] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [ProductSuppliers] ADD CONSTRAINT [FK_ProductSuppliers_Units_PurchaseUnitId] FOREIGN KEY ([PurchaseUnitId]) REFERENCES [Units] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260910214451_AddUnitForeignKeys', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductSuppliers]') AND [c].[name] = N'PurchaseUnit');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ProductSuppliers] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [ProductSuppliers] DROP COLUMN [PurchaseUnit];

DECLARE @var2 nvarchar(max);
SELECT @var2 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PPEProducts]') AND [c].[name] = N'StockUnit');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [PPEProducts] DROP CONSTRAINT ' + @var2 + ';');
ALTER TABLE [PPEProducts] DROP COLUMN [StockUnit];

DROP INDEX [IX_ProductSuppliers_PurchaseUnitId] ON [ProductSuppliers];
DECLARE @var3 nvarchar(max);
SELECT @var3 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductSuppliers]') AND [c].[name] = N'PurchaseUnitId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ProductSuppliers] DROP CONSTRAINT ' + @var3 + ';');
ALTER TABLE [ProductSuppliers] ALTER COLUMN [PurchaseUnitId] int NOT NULL;
CREATE INDEX [IX_ProductSuppliers_PurchaseUnitId] ON [ProductSuppliers] ([PurchaseUnitId]);

DROP INDEX [IX_PPEProducts_StockUnitId] ON [PPEProducts];
DECLARE @var4 nvarchar(max);
SELECT @var4 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PPEProducts]') AND [c].[name] = N'StockUnitId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [PPEProducts] DROP CONSTRAINT ' + @var4 + ';');
ALTER TABLE [PPEProducts] ALTER COLUMN [StockUnitId] int NOT NULL;
CREATE INDEX [IX_PPEProducts_StockUnitId] ON [PPEProducts] ([StockUnitId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260911130829_FinalizeUnitForeignKeys', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [ProductSizes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedByUserId] int NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedByUserId] int NULL,
    CONSTRAINT [PK_ProductSizes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductSizes_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProductSizes_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_ProductSizes_CreatedByUserId] ON [ProductSizes] ([CreatedByUserId]);

CREATE UNIQUE INDEX [IX_ProductSizes_Name] ON [ProductSizes] ([Name]);

CREATE INDEX [IX_ProductSizes_UpdatedByUserId] ON [ProductSizes] ([UpdatedByUserId]);

INSERT INTO ProductSizes
(
    Name,
    IsActive,
    CreatedAt,
    CreatedByUserId,
    UpdatedAt,
    UpdatedByUserId
)
SELECT
    Existing.SizeName,
    1,
    SYSUTCDATETIME(),
    NULL,
    NULL,
    NULL
FROM
(
    SELECT DISTINCT
        LTRIM(RTRIM(Size)) AS SizeName
    FROM PPEProducts
    WHERE NULLIF(LTRIM(RTRIM(Size)), '') IS NOT NULL
) AS Existing
WHERE NOT EXISTS
(
    SELECT 1
    FROM ProductSizes PS
    WHERE PS.Name = Existing.SizeName
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260911150424_AddProductSizesCatalog', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var5 nvarchar(max);
SELECT @var5 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PPEProducts]') AND [c].[name] = N'Size');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [PPEProducts] DROP CONSTRAINT ' + @var5 + ';');
ALTER TABLE [PPEProducts] DROP COLUMN [Size];

ALTER TABLE [PPEProducts] ADD [SizeId] int NULL;

CREATE INDEX [IX_PPEProducts_SizeId] ON [PPEProducts] ([SizeId]);

ALTER TABLE [PPEProducts] ADD CONSTRAINT [FK_PPEProducts_ProductSizes_SizeId] FOREIGN KEY ([SizeId]) REFERENCES [ProductSizes] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260911152340_ReplaceProductSizeWithForeignKey', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var6 nvarchar(max);
SELECT @var6 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PPEProducts]') AND [c].[name] = N'Color');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [PPEProducts] DROP CONSTRAINT ' + @var6 + ';');
ALTER TABLE [PPEProducts] DROP COLUMN [Color];

ALTER TABLE [PPEProducts] ADD [ColorId] int NULL;

CREATE TABLE [ProductColors] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedByUserId] int NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedByUserId] int NULL,
    CONSTRAINT [PK_ProductColors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductColors_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProductColors_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_PPEProducts_ColorId] ON [PPEProducts] ([ColorId]);

CREATE INDEX [IX_ProductColors_CreatedByUserId] ON [ProductColors] ([CreatedByUserId]);

CREATE UNIQUE INDEX [IX_ProductColors_Name] ON [ProductColors] ([Name]);

CREATE INDEX [IX_ProductColors_UpdatedByUserId] ON [ProductColors] ([UpdatedByUserId]);

ALTER TABLE [PPEProducts] ADD CONSTRAINT [FK_PPEProducts_ProductColors_ColorId] FOREIGN KEY ([ColorId]) REFERENCES [ProductColors] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260911165958_AddProductColorsCatalog', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [WarehouseProducts] (
    [WarehouseId] int NOT NULL,
    [PPEProductId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedByUserId] int NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedByUserId] int NULL,
    CONSTRAINT [PK_WarehouseProducts] PRIMARY KEY ([WarehouseId], [PPEProductId]),
    CONSTRAINT [FK_WarehouseProducts_PPEProducts_PPEProductId] FOREIGN KEY ([PPEProductId]) REFERENCES [PPEProducts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_WarehouseProducts_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_WarehouseProducts_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_WarehouseProducts_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_WarehouseProducts_CreatedByUserId] ON [WarehouseProducts] ([CreatedByUserId]);

CREATE INDEX [IX_WarehouseProducts_PPEProductId] ON [WarehouseProducts] ([PPEProductId]);

CREATE INDEX [IX_WarehouseProducts_UpdatedByUserId] ON [WarehouseProducts] ([UpdatedByUserId]);

INSERT INTO WarehouseProducts
(
    WarehouseId,
    PPEProductId,
    IsActive,
    CreatedAt,
    CreatedByUserId,
    UpdatedAt,
    UpdatedByUserId
)
SELECT DISTINCT
    ib.WarehouseId,
    ib.PPEProductId,
    1,
    SYSUTCDATETIME(),
    NULL,
    NULL,
    NULL
FROM InventoryBalances AS ib
WHERE NOT EXISTS
(
    SELECT 1
    FROM WarehouseProducts AS wp
    WHERE wp.WarehouseId = ib.WarehouseId
      AND wp.PPEProductId = ib.PPEProductId
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260915162101_AddWarehouseProducts', N'10.0.11');

COMMIT;
GO

