CREATE TABLE [dbo].[Infra.Service] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [ServiceClassID] INT            NOT NULL,
    [ModelClassID]   INT            NOT NULL,
    [Code]           NVARCHAR (MAX) NULL,
    [MethodName]     NVARCHAR (MAX) NULL,
    [CachingTypeID]  INT            NOT NULL,
    [AccessTypeID]   INT            NOT NULL,
    [Description]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Infra.Service] PRIMARY KEY CLUSTERED ([Id] ASC)
);

