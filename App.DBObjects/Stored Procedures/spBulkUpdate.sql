
Create proc [dbo].[spBulkUpdate]
@xmlData xml
as
begin
begin transaction [txnSetClientLien]
  begin try
	update pp set 
		pp.PhoneNumber = xDT.PhoneNumber,
		pp.PhoneNumberTypeID = xDT.PhoneNumberTypeID,
		pp.ModifiedDate = xDT.ModifiedDate,
		pp.IsActive = xDT.IsActive
	from Person.PersonPhone pp 
	inner join (
		select Tab.Col.value('(@PhoneNumber)[1]','nvarchar(max)') PhoneNumber, 
		Tab.Col.value('(@PhoneNumberTypeID)[1]','int') PhoneNumberTypeID,
		Tab.Col.value('(@BusinessEntityID)[1]','int') BusinessEntityID,
		Tab.Col.value('(@ModifiedDate)[1]','datetime') ModifiedDate,
		Tab.Col.value('(@IsActive)[1]','bit') IsActive
		from  @xmlData.nodes('phones/phone') Tab(Col)
	) xDT
	on pp.BusinessEntityID = xDT.BusinessEntityID
    commit transaction [txnSetClientLien]
  end try
	begin catch
      rollback transaction [txnSetClientLien]
  end catch
end