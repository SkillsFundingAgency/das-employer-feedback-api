-- Database Cleanup: Remove Legacy Tables

IF OBJECT_ID('[dbo].[EmployerSurveyHistory]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[EmployerSurveyHistory]
END

GO

IF OBJECT_ID('[dbo].[EmployerSurveyCodes]', 'U') IS NOT NULL 
BEGIN
    DROP TABLE [dbo].[EmployerSurveyCodes]
END


IF OBJECT_ID('[dbo].[Users]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Users]
END

IF OBJECT_ID('[dbo].[Providers]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Providers]
END

IF OBJECT_ID('[dbo].[EmployerEmailDetails]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[EmployerEmailDetails]
END

IF OBJECT_ID('[dbo].[staging_commitment]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[staging_commitment]
END

IF OBJECT_ID('[dbo].[staging_roatp]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[staging_roatp]
END

IF OBJECT_ID('[dbo].[staging_employer_accounts]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[staging_employer_accounts]
END

PRINT 'Database cleanup: Removed legacy tables'