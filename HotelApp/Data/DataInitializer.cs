using HotelApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Data
{
    public class DataInitializer
    {
        private ApplicationDbContext_FAKE dbContext;

        //public ApplicationDbContext_FAKE MigrateAndSeedData()
        //{
        //    dbContext = new ApplicationDbContext_FAKE();
        //    SeedCustomers();
        //    SeedRooms();
        //    SeedBookings();
        //    SeedInvoices();
        //    SeedBookingRooms();

        //    return dbContext;
        //}
        public void MigrateAndSeedData(ApplicationDbContext_FAKE dbContext)
        {
            SeedCustomers(dbContext);
            SeedRooms(dbContext);
            SeedBookings(dbContext);
            SeedInvoices(dbContext);
            SeedBookingRooms(dbContext);

            // Koppla ihop relationer efter att allt är seedat
            LinkRelationships(dbContext);
        }

        private void SeedCustomers(ApplicationDbContext_FAKE dbContext)
        {
            // Skapa 4 kunder
            var customer1 = new Customer
            {
                CustomerId = 1,
                FirstName = "Anna",
                LastName = "Svensson",
                EmailAddress = "anna.svensson@example.com",
                PhoneNumber = "0701234567",
                DateOfBirth = new DateOnly(1985, 4, 12),
                Address = "Storgatan 10, 111 22 Stockholm",
                Membership = TypeOfMembership.Brons,
                OtherInfoInCustomer = "Har ställt till med bråk i frukortkön vid några tillfällen",
                ListOfBookingsInCustomer = new List<Booking>(),
                IsActive = true,
            };

            var customer2 = new Customer
            {
                CustomerId = 2,
                FirstName = "Erik",
                LastName = "Johansson",
                EmailAddress = "erik.johansson@example.com",
                PhoneNumber = "0707654321",
                DateOfBirth = new DateOnly(1990, 8, 23),
                Address = "Långgatan 20, 111 33 Stockholm",
                Membership = TypeOfMembership.Silver,
                ListOfBookingsInCustomer = new List<Booking>(),
                IsActive = true
            };

            var customer3 = new Customer
            {
                CustomerId = 3,
                FirstName = "Lisa",
                LastName = "Karlsson",
                EmailAddress = "lisa.karlsson@example.com",
                PhoneNumber = "0701122334",
                DateOfBirth = new DateOnly(1992, 11, 5),
                Address = "Västerlånggatan 5, 123 45 Malmö",
                Membership = TypeOfMembership.Guld,
                OtherInfoInCustomer = "En av våra bästa kunder, se till att hon är nöjd",
                ListOfBookingsInCustomer = new List<Booking>(),
                IsActive = true
            };

            var customer4 = new Customer
            {
                CustomerId = 4,
                FirstName = "Johan",
                LastName = "Nilsson",
                EmailAddress = "johan.nilsson@example.com",
                PhoneNumber = "0709876543",
                DateOfBirth = new DateOnly(1988, 2, 19),
                Address = "Torggatan 3, 222 33 Göteborg",
                Membership = TypeOfMembership.Brons,
                ListOfBookingsInCustomer = new List<Booking>(),
                IsActive = true,
            };

            // Lägg till kunder i databasen
            dbContext.Customers.AddRange(new[]{ customer1, customer2, customer3, customer4 });
        }

        private void SeedRooms(ApplicationDbContext_FAKE dbContext)
        {
            // Skapa 4 rum med korrekta MaxPersonsAllowedInRoom
            var room1 = new Room
            {
                RoomNumber = 101,
                RoomType = BedSize.Single,
                NumberOfPossibleExtraBeds = 0,
                CostPerNight = 500m,
                IsDisabilityFriendly = false,
                OtherOrDescription = "Dålig ljudisolering mot poolområde. Planerat underhåll v24.",
                IsActive = true,
                ListOfBookingRoomsInRoom = new List<BookingRoom>()
            };

            var room2 = new Room
            {
                RoomNumber = 102,
                RoomType = BedSize.Double,
                NumberOfPossibleExtraBeds = 1,
                CostPerNight = 800m,
                IsDisabilityFriendly = true,
                OtherOrDescription = "Utsikt mot grå betongvägg. Planerat underhåll v43.",
                IsActive = true,
                ListOfBookingRoomsInRoom = new List<BookingRoom>()
            };

            var room3 = new Room
            {
                RoomNumber = 103,
                RoomType = BedSize.Double,
                NumberOfPossibleExtraBeds = 2,
                CostPerNight = 1200m,
                IsDisabilityFriendly = true,
                OtherOrDescription = "Havsutsikt",
                IsActive = true,
                ListOfBookingRoomsInRoom = new List<BookingRoom>()
            };

            var room4 = new Room
            {
                RoomNumber = 104,
                RoomType = BedSize.Double,
                NumberOfPossibleExtraBeds = 2,
                CostPerNight = 1500m,
                IsDisabilityFriendly = false,
                OtherOrDescription = "Havsutsikt med designmöbler.",
                IsActive = true,
                ListOfBookingRoomsInRoom = new List<BookingRoom>()
            };

            // Lägg till rum i databasen
            dbContext.Rooms.AddRange(new[] { room1, room2, room3, room4 });
        }

        private void SeedBookings(ApplicationDbContext_FAKE dbContext)
        {
            // Skapa bokningar och kontrollera NumberOfGuests mot MaxPersonsAllowedInRoom
            var booking1 = new Booking
            {
                BookingId = 1,
                CustomerId = dbContext.Customers[0].CustomerId,
                CustomerInBooking = dbContext.Customers[0],
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(1),
                NumberOfGuests = 1,
                OtherInfoInBooking = "Önskar sen utcheckning, 14.00",
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            var booking2 = new Booking
            {
                BookingId = 2,
                CustomerId = dbContext.Customers[1].CustomerId,
                CustomerInBooking = dbContext.Customers[1],
                StartDate = DateTime.Now.AddDays(2), 
                EndDate = DateTime.Now.AddDays(6),
                NumberOfGuests = 3, // Valid for room2
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            var booking3 = new Booking
            {
                BookingId = 3,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(7),
                NumberOfGuests = 5,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            var booking4 = new Booking
            {
                BookingId = 4,
                CustomerId = dbContext.Customers[3].CustomerId,
                CustomerInBooking = dbContext.Customers[3],
                StartDate = DateTime.Now.AddDays(4),
                EndDate = DateTime.Now.AddDays(8),
                NumberOfGuests = 6, // Valid for room4
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking5 = new Booking
            {
                BookingId = 5,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(-27),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking6 = new Booking
            {
                BookingId = 6,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(-40),
                EndDate = DateTime.Now.AddDays(-37),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking7 = new Booking
            {
                BookingId = 7,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(-50),
                EndDate = DateTime.Now.AddDays(-48),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking8 = new Booking
            {
                BookingId = 8,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-8),
                NumberOfGuests = 2,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking9 = new Booking
            {
                BookingId = 9,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-12),
                NumberOfGuests = 2,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking10 = new Booking
            {
                BookingId = 10,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(-20),
                EndDate = DateTime.Now.AddDays(-23),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking11 = new Booking
            {
                BookingId = 11,
                CustomerId = dbContext.Customers[2].CustomerId,
                CustomerInBooking = dbContext.Customers[2],
                StartDate = DateTime.Now.AddDays(33),
                EndDate = DateTime.Now.AddDays(35),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            // Lägg till bokningar i databasen
            dbContext.Bookings.AddRange(new[] { booking1, booking2, booking3, booking4, booking5, booking6, booking7, booking8, booking9, booking10, booking11 });
        }

        private void SeedInvoices(ApplicationDbContext_FAKE dbContext)
        {
            // Skapa fakturor
            var invoice1 = new Invoice
            {
                InvoiceId = 1,
                BookingId = dbContext.Bookings[0].BookingId,
                BookingInInvoice = dbContext.Bookings[0],
                TotalAmount = 2000m,
                DueDate = dbContext.Bookings[0].StartDate.AddDays(30),
                InvoiceDate = dbContext.Bookings[0].StartDate,
                IsOverDue = false,
                IsPaid = true
            };

            var invoice2 = new Invoice
            {
                InvoiceId = 2,
                BookingId = dbContext.Bookings[1].BookingId,
                BookingInInvoice = dbContext.Bookings[1],
                TotalAmount = 3200m,
                DueDate = dbContext.Bookings[1].StartDate.AddDays(20),
                InvoiceDate = dbContext.Bookings[1].StartDate,
                IsOverDue = false,
                IsPaid = true
            };

            var invoice3 = new Invoice
            {
                InvoiceId = 3,
                BookingId = dbContext.Bookings[2].BookingId,
                BookingInInvoice = dbContext.Bookings[2],
                TotalAmount = 3600m,
                DueDate = dbContext.Bookings[2].StartDate.AddDays(10),
                InvoiceDate = dbContext.Bookings[2].StartDate,
                IsOverDue = false,
                IsPaid = false
            };

            var invoice4 = new Invoice
            {
                InvoiceId = 4,
                BookingId = dbContext.Bookings[3].BookingId,
                BookingInInvoice = dbContext.Bookings[3],
                TotalAmount = 4000m,
                DueDate = dbContext.Bookings[3].StartDate.AddDays(30),
                InvoiceDate = dbContext.Bookings[3].StartDate,
                IsOverDue = true,
                IsPaid = false
            };

            // Lägg till fakturor i databasen
            dbContext.Invoices.AddRange(new[] { invoice1, invoice2, invoice3, invoice4 });
        }

        private void SeedBookingRooms(ApplicationDbContext_FAKE dbContext)
        {
            // Kontrollera att Bookings och Rooms har seedats
            if (dbContext.Bookings.Count == 0 || dbContext.Rooms.Count == 0)
            {
                throw new InvalidOperationException("Bookings or Rooms lists are empty. Please seed them first.");
            }

            var bookingRoom1 = new BookingRoom
            {
                BookingId = dbContext.Bookings[0].BookingId,
                Booking = dbContext.Bookings[0],
                RoomNumberAsID = dbContext.Rooms[0].RoomNumber,
                Room = dbContext.Rooms[0],
            };

            var bookingRoom2 = new BookingRoom
            {
                BookingId = dbContext.Bookings[1].BookingId,
                Booking = dbContext.Bookings[1],
                RoomNumberAsID = dbContext.Rooms[1].RoomNumber,
                Room = dbContext.Rooms[1],
            };

            var bookingRoom3 = new BookingRoom
            {
                BookingId = dbContext.Bookings[2].BookingId,
                Booking = dbContext.Bookings[2],
                RoomNumberAsID = dbContext.Rooms[2].RoomNumber,
                Room = dbContext.Rooms[2],
            };

            var bookingRoom4 = new BookingRoom
            {
                BookingId = dbContext.Bookings[3].BookingId,
                Booking = dbContext.Bookings[3],
                RoomNumberAsID = dbContext.Rooms[3].RoomNumber,
                Room = dbContext.Rooms[3],
            };

            dbContext.BookingRooms.AddRange(new[] { bookingRoom1, bookingRoom2, bookingRoom3, bookingRoom4 });
        }

        private void LinkRelationships(ApplicationDbContext_FAKE dbContext)
        {
            // Koppla BookingRooms till respektive Room och Booking
            foreach (var bookingRoom in dbContext.BookingRooms)
            {
                var room = dbContext.Rooms.FirstOrDefault(r => r.RoomNumber == bookingRoom.RoomNumberAsID);
                var booking = dbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingRoom.BookingId);

                if (room != null)
                {
                    room.ListOfBookingRoomsInRoom.Add(bookingRoom);
                }

                if (booking != null)
                {
                    booking.ListOfBookingRoomsInBooking.Add(bookingRoom);
                }
            }
            foreach (var booking in dbContext.Bookings)
            {
                var customer = dbContext.Customers.FirstOrDefault(c => c.CustomerId == booking.CustomerId);

                if (customer != null)
                {
                    customer.ListOfBookingsInCustomer.Add(booking);
                }
            }
            
        }
    }
}
