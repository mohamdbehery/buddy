
Create procedure [dbo].[spGetXMLRows]
@BEID int
as
begin
select(
select 
isnull(p.BusinessEntityID, 0) BusinessEntityID, 
isnull(p.PersonType, '') PersonType, 
isnull(p.Title, '') Title, 
isnull(p.FirstName, '') FirstName, 
isnull(p.MiddleName, '') MiddleName, 
isnull(p.LastName, '') LastName, 
isnull(p.rowguid, '') rowguid, 
isnull(p.ModifiedDate, '') ModifiedDate,
dbo.fGetXMLRowsByID(p.BusinessEntityID)
from Person.Person p
where p.BusinessEntityID > @BEID
for xml raw('person'), root('persons')
) xmlData
end