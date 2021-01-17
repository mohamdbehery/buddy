CREATE procedure [dbo].[spStartMessageExecution]
@MessageID int
as
update [Demo.MQMessage] set ExecuteDate = GETDATE()
where Id = @MessageID
