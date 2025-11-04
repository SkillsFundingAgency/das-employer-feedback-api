-- Database Cleanup: Remove Legacy Tables: EmployerFeedback Cleanup

-- Remove orphaned EmployerFeedback records that do not have associated feedback results
EXEC('
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''EmployerFeedback'' AND TABLE_SCHEMA = ''dbo'')
BEGIN
    DELETE ef
    FROM [dbo].[EmployerFeedback] ef
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[EmployerFeedbackResult] efr 
        WHERE efr.FeedbackId = ef.FeedbackId
    )
END
')
GO

 -- Drop the default constraint first
EXEC('
IF COL_LENGTH(''dbo.EmployerFeedback'', ''IsActive'') IS NOT NULL
BEGIN
    -- Find and drop the default constraint on IsActive column
    DECLARE @DefaultConstraintName NVARCHAR(200)
    SELECT @DefaultConstraintName = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
    INNER JOIN sys.objects o ON dc.parent_object_id = o.object_id
    WHERE o.name = ''EmployerFeedback'' AND c.name = ''IsActive''
    

    IF @DefaultConstraintName IS NOT NULL
    BEGIN
        DECLARE @DropConstraintSQL NVARCHAR(500) = ''ALTER TABLE [dbo].[EmployerFeedback] DROP CONSTRAINT ['' + @DefaultConstraintName + '']''
        EXEC sp_executesql @DropConstraintSQL
    END
END
')
GO

-- Drop IsActive column if it exists
EXEC('IF COL_LENGTH(''dbo.EmployerFeedback'', ''IsActive'') IS NOT NULL ALTER TABLE [dbo].[EmployerFeedback] DROP COLUMN [IsActive]')
GO

PRINT 'EmployerFeedback Cleanup: Completed cleanup operations'
GO