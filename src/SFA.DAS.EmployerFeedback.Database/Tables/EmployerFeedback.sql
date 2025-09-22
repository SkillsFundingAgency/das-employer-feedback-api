CREATE TABLE [dbo].[EmployerFeedback]
(
	[FeedbackId] BIGINT NOT NULL PRIMARY KEY IDENTITY,
	[UserRef] UNIQUEIDENTIFIER NOT NULL,
	[Ukprn] BIGINT NOT NULL,
	[AccountId] BIGINT NOT NULL,
	[IsActive] BIT NOT NULL DEFAULT 0
)

GO

CREATE UNIQUE INDEX [IXU_EmployerFeedback_UserRef_Ukprn_AccountId] ON [dbo].[EmployerFeedback] ([UserRef], [Ukprn], [AccountId]) WITH (ONLINE = ON)

