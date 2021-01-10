CREATE TABLE [dbo].[Infra.User] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]       NVARCHAR (MAX) NULL,
    [SecondName]      NVARCHAR (MAX) NULL,
    [FamilyName]      NVARCHAR (MAX) NULL,
    [BirthDate]       DATETIME2 (7)  NOT NULL,
    [eMailAddress]    NVARCHAR (MAX) NULL,
    [LocationAddress] NVARCHAR (MAX) NULL,
    [UserName]        NVARCHAR (MAX) NULL,
    [Password]        NVARCHAR (MAX) NULL,
    [EntryDate]       DATETIME2 (7)  NOT NULL,
    [ModifyDate]      DATETIME2 (7)  NULL,
    [IsActive]        BIT            NOT NULL,
    [IsDeleted]       BIT            NOT NULL,
    CONSTRAINT [PK_Infra.User] PRIMARY KEY CLUSTERED ([Id] ASC)
);

