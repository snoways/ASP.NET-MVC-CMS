/****** Object:  Table [dbo].[CMS_WidgetBase]    Script Date: 2016/6/1 星期三 16:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CMS_WidgetBase](
	[ID] [nvarchar](100) NOT NULL,
	[WidgetName] [nvarchar](255) NULL,
	[Title] [nvarchar](255) NULL,
	[Position] [int] NULL,
	[LayoutId] [nvarchar](100) NULL,
	[PageId] [nvarchar](100) NULL,
	[ZoneId] [nvarchar](100) NULL,
	[PartialView] [nvarchar](255) NULL,
	[AssemblyName] [nvarchar](255) NULL,
	[ServiceTypeName] [nvarchar](255) NULL,
	[ViewModelTypeName] [nvarchar](255) NULL,
	[FormView] [nvarchar](255) NULL,
	[StyleClass] [nvarchar](1000) NULL,
	[CreateBy] [nvarchar](255) NULL,
	[CreatebyName] [nvarchar](255) NULL,
	[CreateDate] [datetime] NULL,
	[LastUpdateBy] [nvarchar](255) NULL,
	[LastUpdateByName] [nvarchar](255) NULL,
	[LastUpdateDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[IsTemplate] [bit] NULL CONSTRAINT [DF__CMS_Widge__IsTem__1B0907CE]  DEFAULT ((0)),
	[Thumbnail] [nvarchar](200) NULL,
	[IsSystem] [bit] NULL CONSTRAINT [DF__CMS_Widge__IsSys__1BFD2C07]  DEFAULT ((0)),
 CONSTRAINT [PK_CMS_WidgetBase] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
