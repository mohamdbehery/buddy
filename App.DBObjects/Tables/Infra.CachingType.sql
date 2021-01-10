CREATE TABLE [dbo].[Infra.CachingType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Infra.CachingType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

