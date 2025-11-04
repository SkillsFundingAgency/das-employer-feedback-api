-- Database Cleanup: Remove Unused Stored Procedures and Views

-- Remove stored procedures
EXEC('IF OBJECT_ID(''[dbo].[CreateEmployerFeedbackResult]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[CreateEmployerFeedbackResult]')
GO
EXEC('IF OBJECT_ID(''[dbo].[UpsertFeedback]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[UpsertFeedback]')
GO
EXEC('IF OBJECT_ID(''[dbo].[GetEmployerSurveyHistory]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[GetEmployerSurveyHistory]')
GO
EXEC('IF OBJECT_ID(''[dbo].[GetSurveyInvitesToSend]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[GetSurveyInvitesToSend]')
GO
EXEC('IF OBJECT_ID(''[dbo].[GetSurveyRemindersToSend]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[GetSurveyRemindersToSend]')
GO
EXEC('IF OBJECT_ID(''[dbo].[GetLatestFeedbackInviteSentDate]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[GetLatestFeedbackInviteSentDate]')
GO
EXEC('IF OBJECT_ID(''[dbo].[UpsertProviders]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[UpsertProviders]')
GO
EXEC('IF OBJECT_ID(''[dbo].[UpsertUsers]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[UpsertUsers]')
GO
EXEC('IF OBJECT_ID(''[dbo].[ResetFeedback]'', ''P'') IS NOT NULL DROP PROCEDURE [dbo].[ResetFeedback]')
GO

-- Remove views 
EXEC('IF OBJECT_ID(''[dbo].[vw_EmployerSurveyHistoryComplete]'', ''V'') IS NOT NULL DROP VIEW [dbo].[vw_EmployerSurveyHistoryComplete]')
GO
EXEC('IF OBJECT_ID(''[dbo].[vw_EmployerSurveyInvites]'', ''V'') IS NOT NULL DROP VIEW [dbo].[vw_EmployerSurveyInvites]')
GO
EXEC('IF OBJECT_ID(''[dbo].[vw_FeedbackToSend]'', ''V'') IS NOT NULL DROP VIEW [dbo].[vw_FeedbackToSend]')
GO

PRINT 'Database cleanup: Removed unused stored procedures and views'
