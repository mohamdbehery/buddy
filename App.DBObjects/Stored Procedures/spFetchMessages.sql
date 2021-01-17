CREATE procedure [dbo].[spFetchMessages]
as
begin transaction [txnFetchMessages]
  begin try
	-- select the non picked up messages before OR the failed meessages since 12 hours or more
	select top(5) * into #temp from [Demo.MQMessage] 
	where (IsActive = 1 AND QueueDate is null AND MSBatchID is null) OR 
	(FailureDate is not null AND datediff(hour, FailureDate, getdate()) >= 12)

	update [Demo.MQMessage] 
	set QueueDate = getdate(), 
	MSBatchID = concat(newid(),'^',t.id)
	from #temp t
	where [Demo.MQMessage].Id = t.Id

	select * from #temp

	IF OBJECT_ID('tempdb..#temp') IS NOT NULL DROP TABLE #temp
	commit transaction [txnFetchMessages]

  end try
	begin catch
      rollback transaction [txnFetchMessages]
  end catch