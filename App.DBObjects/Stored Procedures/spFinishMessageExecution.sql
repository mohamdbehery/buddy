CREATE procedure [dbo].[spFinishMessageExecution]
@MessageID int,
@MessageData nvarchar(max)
as

insert into [Demo.MQExecution] (MessageID, ExecutionResult, SuccessDate)
values (@MessageID, @MessageData, getdate())

update [Demo.MQMessage] set MessageData = @MessageData, SuccessDate = GETDATE(), FailureDate = null, FailureMessage  =null 
where MessageID = @MessageID
