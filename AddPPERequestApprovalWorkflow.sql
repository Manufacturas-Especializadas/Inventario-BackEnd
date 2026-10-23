BEGIN TRANSACTION;
ALTER TABLE [PPERequests] ADD [ApprovedAt] datetime2 NULL;

ALTER TABLE [PPERequests] ADD [ApprovedByUserId] int NULL;

ALTER TABLE [PPERequests] ADD [RejectedAt] datetime2 NULL;

ALTER TABLE [PPERequests] ADD [RejectedByUserId] int NULL;

ALTER TABLE [PPERequests] ADD [RejectionReason] nvarchar(500) NULL;

CREATE INDEX [IX_PPERequests_ApprovedByUserId] ON [PPERequests] ([ApprovedByUserId]);

CREATE INDEX [IX_PPERequests_RejectedByUserId] ON [PPERequests] ([RejectedByUserId]);

ALTER TABLE [PPERequests] ADD CONSTRAINT [FK_PPERequests_Users_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [PPERequests] ADD CONSTRAINT [FK_PPERequests_Users_RejectedByUserId] FOREIGN KEY ([RejectedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261019201523_AddPPERequestApprovalWorkflow', N'10.0.11');

COMMIT;
GO

