CREATE TABLE [dbo].[Account]
(
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,
    [AccountName] NVARCHAR(255) NOT NULL,
    [CheckedOn] DATETIME2 NULL
)
GO

CREATE INDEX [IX_Account_CheckedOn] ON [dbo].[Account] (CheckedOn)