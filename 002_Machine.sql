ALTER TABLE [dbo].[Machine] DROP CONSTRAINT [FK_Machine_Plant];
GO

ALTER TABLE [dbo].[Machine] ALTER COLUMN [PlantId] INT NULL;
GO

ALTER TABLE [dbo].[Machine] WITH NOCHECK
    ADD CONSTRAINT [FK_Machine_Plant] FOREIGN KEY ([PlantId]) REFERENCES [dbo].[Plant] ([Id]);
GO