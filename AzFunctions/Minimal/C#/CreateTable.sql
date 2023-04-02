/****** Object:  Table [stg].[IMDB]    Script Date: 02/04/2023 13:44:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[stg].[IMDB]') AND type in (N'U'))
DROP TABLE [stg].[IMDB]
GO

/****** Object:  Table [stg].[IMDB]    Script Date: 02/04/2023 13:44:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [stg].[IMDB]
(
	[movie title] [varchar](4000) NULL,
	[Run Time] [varchar](4000) NULL,
	[Rating] [varchar](4000) NULL,
	[User Rating] [varchar](4000) NULL,
	[Generes] [varchar](4000) NULL,
	[Overview] [varchar](4000) NULL,
	[Plot Kyeword] [varchar](4000) NULL,
	[Director] [varchar](4000) NULL,
	[Top 5 Casts] [varchar](4000) NULL,
	[Writer] [varchar](4000) NULL,
	[year] [varchar](4000) NULL,
	[path] [varchar](4000) NULL
)
WITH
(
	DISTRIBUTION = ROUND_ROBIN,
	HEAP
)
GO


