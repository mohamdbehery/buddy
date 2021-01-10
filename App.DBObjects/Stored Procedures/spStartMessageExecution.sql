CREATE procedure [dbo].[spStartMessageExecution]
@MessageID int
as
declare @MSBatchID nvarchar(max) = concat(newid(),'^',@messageid);
update [Demo.MQMessage] set ExecuteDate = GETDATE(), MSBatchID = @MSBatchID
where MessageID = @MessageID
