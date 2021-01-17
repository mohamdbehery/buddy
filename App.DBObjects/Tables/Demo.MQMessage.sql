CREATE TABLE [dbo].[Demo.MQMessage] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [QueueDate]      DATETIME2 (7)  NULL,
    [MessageData]    NVARCHAR (MAX) NULL,
    [ExecuteDate]    DATETIME2 (7)  NULL,
    [MSBatchID]      NVARCHAR (MAX) NULL,
    [SuccessDate]    DATETIME2 (7)  NULL,
    [FailureDate]    DATETIME2 (7)  NULL,
    [FailureMessage] NVARCHAR (MAX) NULL,
    [IsActive]       BIT            NOT NULL,
    CONSTRAINT [PK_Demo.MQMessage] PRIMARY KEY CLUSTERED ([Id] ASC)
);

