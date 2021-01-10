CREATE TABLE [dbo].[Infra.AccessType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Infra.AccessType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

