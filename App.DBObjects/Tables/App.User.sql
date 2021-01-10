CREATE TABLE [dbo].[App.User] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]       NVARCHAR (100) NOT NULL,
    [SecondName]      NVARCHAR (100) NOT NULL,
    [FamilyName]      NVARCHAR (100) NOT NULL,
    [EMailAddress]    NVARCHAR (200) NOT NULL,
    [LocationAddress] NVARCHAR (300) NULL,
    [UserName]        NVARCHAR (200) NULL,
    [Password]        VARCHAR (500)  NULL,
    [EntryDate]       DATETIME       NOT NULL,
    [ModifyDate]      DATETIME       NULL,
    [IsActive]        BIT            NOT NULL,
    [IsDeleted]       BIT            NOT NULL,
    CONSTRAINT [PK_App.User] PRIMARY KEY CLUSTERED ([Id] ASC)
);

