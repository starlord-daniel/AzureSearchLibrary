DROP TABLE [dbo].[qna_follow_up];

CREATE TABLE [dbo].[qna_follow_up](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[response] [nvarchar](max) NOT NULL,
    [options] [nvarchar](max) NULL,
	[follow_up] [nvarchar](max) NULL,
	[feedback_score] [int] NULL,
	[comments] [nvarchar](max) NULL
)