CREATE TABLE [dbo].[Infra.Class] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [AssemblyID]  INT            NOT NULL,
    [Name]        NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Infra.Class] PRIMARY KEY CLUSTERED ([Id] ASC)
);

