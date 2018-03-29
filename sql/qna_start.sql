DROP TABLE [dbo].[qna_start];

CREATE TABLE [dbo].[qna_start](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[question] [nvarchar](max) NOT NULL,
    [answer] [nvarchar](max) NULL,
	[properties] [nvarchar](max) NULL,
	[category] [nvarchar](max) NULL,
	[follow_up] [nvarchar](max) NULL,
	[feedback_score] [int] NULL,
	[comments] [nvarchar](max) NULL    
)