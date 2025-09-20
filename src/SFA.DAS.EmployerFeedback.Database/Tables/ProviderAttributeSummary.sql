CREATE TABLE [dbo].[ProviderAttributeSummary](
	[Ukprn] [bigint] NOT NULL,
	[AttributeId] [bigint] NOT NULL,
	[Strength] [int] NOT NULL,
	[Weakness] [int] NOT NULL,
	[TimePeriod] NVARCHAR(50) NOT NULL DEFAULT 'All',
	[UpdatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Ukprn] ASC,
	[AttributeId] ASC,
	[TimePeriod] ASC
))

GO

ALTER TABLE [dbo].[ProviderAttributeSummary] ADD  DEFAULT ((0)) FOR [Strength]
GO

ALTER TABLE [dbo].[ProviderAttributeSummary] ADD  DEFAULT ((0)) FOR [Weakness]
GO
