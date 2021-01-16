CREATE procedure [dbo].[spFetchMessages]
as

select * into #temp from [Demo.MQMessage] where IsActive = 1 and FetchDate is null and QueueDate is null or FailureDate is not null or MSBatchID is null

update [Demo.MQMessage] set FetchDate = getdate(), QueueDate = getdate() where IsActive = 1 and FetchDate is null and QueueDate is null

select * from #temp

IF OBJECT_ID('tempdb..#temp') IS NOT NULL DROP TABLE #temp
