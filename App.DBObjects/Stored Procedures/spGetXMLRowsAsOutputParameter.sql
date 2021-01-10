
Create procedure [dbo].[spGetXMLRowsAsOutputParameter]
@BusinessEntityID int,
@Phones xml output
as
begin
        set @Phones = (
		select
		isnull(pp.PhoneNumber, '') PhoneNumber,
		isnull(pp.PhoneNumberTypeID, '') PhoneNumberTypeID,
		isnull(pp.ModifiedDate, '') ModifiedDate
		from Person.PersonPhone pp 
		where pp.BusinessEntityID = @BusinessEntityID
		for xml raw('phone')
    )
end