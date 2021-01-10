
Create proc [dbo].[spBulkInsert]
@xmlData xml
as
begin
begin transaction [txnSetClientLien]
  begin try

	Insert into Person.PersonPhone (PhoneNumber,PhoneNumberTypeID,BusinessEntityID,ModifiedDate,IsActive)
	select * from (		
		select Tab.Col.value('(@PhoneNumber)[1]','nvarchar(max)') PhoneNumber, 
		Tab.Col.value('(@PhoneNumberTypeID)[1]','int') PhoneNumberTypeID,
		Tab.Col.value('(@BusinessEntityID)[1]','int') BusinessEntityID,
		Tab.Col.value('(@ModifiedDate)[1]','datetime') ModifiedDate,
		Tab.Col.value('(@IsActive)[1]','bit') IsActive
		from  @xmlData.nodes('phones/phone') Tab(Col)
	) temp
    commit transaction [txnSetClientLien]

  end try
	begin catch
      rollback transaction [txnSetClientLien]
  end catch
end