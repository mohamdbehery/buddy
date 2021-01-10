
Create function [dbo].[fGetXMLRowsByID](@key as int)
returns xml
begin
    return (
        select
		isnull(pp.PhoneNumber, '') PhoneNumber,
		isnull(pp.PhoneNumberTypeID, '') PhoneNumberTypeID,
		isnull(pp.ModifiedDate, '') ModifiedDate
		from Person.PersonPhone pp 
		where pp.BusinessEntityID = @key
		for xml raw('phone')
    )
end