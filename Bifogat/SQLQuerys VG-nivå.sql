--VG-nivå:
----------
--Visa BokningsNr och Kundnamn på de kunder som har incheckning i december:

USE LakeVerdictResort
SELECT 
    b.Id AS BookingNr,
	CONCAT(c.FirstName , ' '  , c.LastName ) AS 'Decemberkunder'
FROM 
    Bookings b
JOIN
	Customers c ON b.CustomerId = c.Id
WHERE 
    MONTH(StartDate) = 12
---------

--Visa kundnamn och antal gäster på de bokningar som har fler än 2 gäster.

SELECT 
	CONCAT(c.FirstName , ' '  , c.LastName ) AS 'Kund',
    b.Id AS 'Antal gäster'
FROM 
    Bookings b
JOIN
	Customers c ON b.CustomerId = c.Id
WHERE 
    b.NumberOfGuests > 2

----------
--Visa namn på de kunder, som har fakturor som inte är betalda inkl fakturanummer


SELECT 
    CONCAT(c.FirstName, ' ', c.LastName) AS KundNamn,
    i.Id AS 'Icke betald FakturaNr'
FROM 
    Invoices i
JOIN 
    Customers c ON i.BookingId = c.Id
WHERE 
    i.IsPaid = 0
----------
--Visa alla rum som är dyrare än genomsnittet:

SELECT
	*
FROM 
	Rooms r 
WHERE r.CostPerNight > (SELECT AVG(CostPerNight) FROM Rooms) 
ORDER BY 
	CostPerNight

----------------------------
-- Visa alla rum som inte är bokade för tillfället.

SELECT 
    r.RoomNumber
FROM 
    Rooms r
WHERE 
    r.Id NOT IN (
	SELECT 
		br.RoomId
	FROM 
		BookingRooms br
	JOIN 
		Bookings b ON br.BookingId = b.Id
    WHERE 
		b.StartDate <= GETDATE() AND b.EndDate >= GETDATE()
    )