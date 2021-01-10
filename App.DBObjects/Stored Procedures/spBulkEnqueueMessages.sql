CREATE proc [dbo].[spBulkEnqueueMessages]
@Messages xml
as
begin
begin transaction [txnEnqueue]
  begin try

	Insert into [Demo.MQMessage](MessageData,IsActive)
	select * from (		
		select Tab.Col.value('(@Data)[1]','nvarchar(max)') MessageData,
		Tab.Col.value('(@IsActive)[1]','bit') IsActive
		from  @Messages.nodes('Messages/Message') Tab(Col)
	) temp
    commit transaction [txnEnqueue]

  end try
	begin catch
      rollback transaction [txnEnqueue]
  end catch
end
