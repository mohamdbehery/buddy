CREATE procedure [dbo].[spUpdateMessageFailure]
@MessageID int,
@FailureMessage nvarchar(max)
as

update [Demo.MQMessage] set FailureDate = GETDATE(), FailureMessage  =@FailureMessage 
where Id = @MessageID
