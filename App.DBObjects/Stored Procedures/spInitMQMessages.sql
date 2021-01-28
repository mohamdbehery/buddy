Create proc [dbo].[spInitMQMessages]
as
begin

truncate table [Demo.MQExecution]
truncate table [Demo.MQMessage]

select * from [Demo.MQMessage]
select * from [Demo.MQExecution]
end