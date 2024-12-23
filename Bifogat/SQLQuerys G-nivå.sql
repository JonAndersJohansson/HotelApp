
-- G-nivå
--Visa namn och kundNr på alla "Guldkunder"

USE LakeVerdictResort;

SELECT 
	c.Id AS KundNr,
	CONCAT(c.FirstName , ' '  , c.LastName ) AS 'Namn på GuldKund'
FROM 
    Customers c
WHERE
    Membership = '2'
ORDER BY
    c.LastName;

----------

-- Visa Rumsnumer på alla dubbelrum:

SELECT 
    r.RoomNumber
FROM 
    Rooms r
WHERE 
    RoomType = 1

----------
-- Visar bokningsnummer på de bokningar som har fler än 2 gäster:

SELECT 
    b.Id AS 'BokingsNr med fler än 2 gäster'
FROM 
    Bookings b
WHERE 
    b.NumberOfGuests > 2

---------
-- Visa alla fakturor som är annulerade

SELECT 
    i.Id AS 'FakturaNr'
FROM 
    Invoices i
WHERE 
    i.IsCancelled = 1