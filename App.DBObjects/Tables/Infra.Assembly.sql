CREATE TABLE [dbo].[Infra.Assembly] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Infra.Assembly] PRIMARY KEY CLUSTERED ([Id] ASC)
);

