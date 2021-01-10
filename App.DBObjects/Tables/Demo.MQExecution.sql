CREATE TABLE [dbo].[Demo.MQExecution] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [MessageID]       INT            NOT NULL,
    [ExecutionResult] NVARCHAR (MAX) NULL,
    [SuccessDate]     DATETIME2 (7)  NOT NULL,
    [IsActive]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Demo.MQExecution] PRIMARY KEY CLUSTERED ([Id] ASC)
);

