CREATE TABLE [dbo].[Reading](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] varchar(20) NOT NULL,
	[DateTimeUtc] [datetime2] NOT NULL DEFAULT getdate(),
	[TemperatureInCelcius] [decimal](8, 2) NOT NULL,
	[Humidity] [decimal](8, 2) NOT NULL,
	[DateTimestampUtc] [datetime2] NOT NULL DEFAULT getdate(),
	[InsertedDateTimestampUtc] [datetime2] NOT NULL DEFAULT getdate(),
	[ReadingDateTimestampUtc] [datetime2] NOT NULL DEFAULT getdate()
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Reading] ADD  CONSTRAINT [PK_Reading] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
