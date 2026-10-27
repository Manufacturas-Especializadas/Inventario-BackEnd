BEGIN TRANSACTION;
CREATE TABLE [UserManagedOrganizationalUnits] (
    [UserId] int NOT NULL,
    [OrganizationalUnitId] int NOT NULL,
    CONSTRAINT [PK_UserManagedOrganizationalUnits] PRIMARY KEY ([UserId], [OrganizationalUnitId]),
    CONSTRAINT [FK_UserManagedOrganizationalUnits_OrganizationalUnits_OrganizationalUnitId] FOREIGN KEY ([OrganizationalUnitId]) REFERENCES [OrganizationalUnits] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserManagedOrganizationalUnits_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_UserManagedOrganizationalUnits_OrganizationalUnitId] ON [UserManagedOrganizationalUnits] ([OrganizationalUnitId]);

INSERT INTO [UserManagedOrganizationalUnits] ([UserId], [OrganizationalUnitId])
SELECT u.[Id], e.[OrganizationalUnitId]
FROM [Users] u
INNER JOIN [Employees] e ON e.[Id] = u.[EmployeeId]
INNER JOIN [OrganizationalUnits] ou ON ou.[Id] = e.[OrganizationalUnitId]
WHERE e.[OrganizationalUnitId] IS NOT NULL
  AND EXISTS (
      SELECT 1 FROM [UserRoles] ur
      INNER JOIN [Roles] r ON r.[Id] = ur.[RoleId]
      WHERE ur.[UserId] = u.[Id] AND r.[Name] = 'Production'
  )
  AND NOT EXISTS (
      SELECT 1 FROM [UserManagedOrganizationalUnits] managed
      WHERE managed.[UserId] = u.[Id]
        AND managed.[OrganizationalUnitId] = e.[OrganizationalUnitId]
  );

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261027144613_AddUserManagedOrganizationalUnits', N'10.0.11');

COMMIT;
GO

