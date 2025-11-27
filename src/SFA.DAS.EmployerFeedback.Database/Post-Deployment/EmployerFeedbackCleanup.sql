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

PRINT 'EmployerFeedback Cleanup: Completed cleanup operations'
GO