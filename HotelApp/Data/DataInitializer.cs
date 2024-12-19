using HotelApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Data
{
    public class DataInitializer
    {
        private ApplicationDbContext _dbContext;
        public DataInitializer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void MigrateAndSeedData()
        {
            SeedCustomers();
            SeedRooms();
            SeedBookings();
            SeedInvoices();
            SeedBookingRooms();
            LinkRelationships();
            _dbContext.SaveChanges();
        }

        private void SeedCustomers()
        {
            // Skapa 4 kunder
            var customer1 = new Customer
            {
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
            _dbContext.Customers.AddRange(customer1, customer2, customer3, customer4);
            _dbContext.SaveChanges();
        }

        private void SeedRooms()
        {
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

            _dbContext.Rooms.AddRange(room1, room2, room3, room4);
            _dbContext.SaveChanges();
        }

        private void SeedBookings()
        {
            var customer1 = _dbContext.Customers.First(c => c.FirstName == "Anna");
            var booking1 = new Booking
            {
                CustomerId = customer1.Id,
                CustomerInBooking = customer1,
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(4),
                NumberOfGuests = 1,
                OtherInfoInBooking = "Önskar sen utcheckning, 14.00",
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            var customer2 = _dbContext.Customers.First(c => c.FirstName == "Erik");
            var booking2 = new Booking
            {
                CustomerId = customer2.Id,
                CustomerInBooking = customer2,
                StartDate = DateTime.Now.AddDays(-1), 
                EndDate = DateTime.Now.AddDays(6),
                NumberOfGuests = 3, // Valid for room2
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            var customer3 = _dbContext.Customers.First(c => c.FirstName == "Lisa");
            var booking3 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(7),
                NumberOfGuests = 5,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            var customer4 = _dbContext.Customers.First(c => c.FirstName == "Johan");
            var booking4 = new Booking
            {
                CustomerId = customer4.Id,
                CustomerInBooking = customer4,
                StartDate = DateTime.Now.AddDays(4),
                EndDate = DateTime.Now.AddDays(8),
                NumberOfGuests = 6, // Valid for room4
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            var booking5 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(-27),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking6 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(-40),
                EndDate = DateTime.Now.AddDays(-37),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking7 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(-50),
                EndDate = DateTime.Now.AddDays(-48),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking8 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-8),
                NumberOfGuests = 2,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking9 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-12),
                NumberOfGuests = 2,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking10 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(-20),
                EndDate = DateTime.Now.AddDays(-23),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };
            var booking11 = new Booking
            {
                CustomerId = customer3.Id,
                CustomerInBooking = customer3,
                StartDate = DateTime.Now.AddDays(33),
                EndDate = DateTime.Now.AddDays(35),
                NumberOfGuests = 1,
                IsCancelled = false,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            _dbContext.Bookings.AddRange(booking1, booking2, booking3, booking4, booking5, booking6, booking7, booking8, booking9, booking10, booking11);
            _dbContext.SaveChanges();
        }

        private void SeedInvoices()
        {
            var customer1 = _dbContext.Customers.First(c => c.FirstName == "Anna");
            var booking1 = _dbContext.Bookings.FirstOrDefault(b => b.CustomerId == customer1.Id);
            var invoice1 = new Invoice
            {
                BookingId = booking1.Id,
                BookingInInvoice = booking1,
                TotalAmount = 2000m,
                DueDate = booking1.StartDate.AddDays(30),
                InvoiceDate = booking1.StartDate,
                IsOverDue = false,
                IsPaid = true
            };

            var customer2 = _dbContext.Customers.First(c => c.FirstName == "Erik");
            var booking2 = _dbContext.Bookings.FirstOrDefault(b => b.CustomerId == customer2.Id);
            var invoice2 = new Invoice
            {
                BookingId = booking2.Id,
                BookingInInvoice = booking2,
                TotalAmount = 3200m,
                DueDate = booking2.StartDate.AddDays(20),
                InvoiceDate = booking2.StartDate,
                IsOverDue = false,
                IsPaid = true
            };

            var customer3 = _dbContext.Customers.First(c => c.FirstName == "Lisa");
            var booking3 = _dbContext.Bookings.FirstOrDefault(b => b.CustomerId == customer3.Id);
            var invoice3 = new Invoice
            {
                BookingId = booking3.Id,
                BookingInInvoice = booking3,
                TotalAmount = 3600m,
                DueDate = booking3.StartDate.AddDays(10),
                InvoiceDate = booking3.StartDate,
                IsOverDue = false,
                IsPaid = false
            };

            var customer4 = _dbContext.Customers.First(c => c.FirstName == "Johan");
            var booking4 = _dbContext.Bookings.FirstOrDefault(b => b.CustomerId == customer4.Id);
            var invoice4 = new Invoice
            {
                BookingId = booking4.Id,
                BookingInInvoice = booking4,
                TotalAmount = 4000m,
                DueDate = booking4.StartDate.AddDays(30),
                InvoiceDate = booking4.StartDate,
                IsOverDue = true,
                IsPaid = false
            };

            _dbContext.Invoices.AddRange(invoice1, invoice2, invoice3, invoice4);
            _dbContext.SaveChanges();
        }

        private void SeedBookingRooms()
        {
            if (!_dbContext.Bookings.Any() || !_dbContext.Rooms.Any())
            {
                throw new InvalidOperationException("Bookings or Rooms lists are empty. Please seed them first.");
            }

            // Skapa BookingRoom-objekt
            var bookingRoom1 = new BookingRoom
            {
                BookingId = _dbContext.Bookings.First().Id,
                Booking = _dbContext.Bookings.First(),
                RoomId = _dbContext.Rooms.First().Id,
                Room = _dbContext.Rooms.First()
            };

            var bookingRoom2 = new BookingRoom
            {
                BookingId = _dbContext.Bookings.Skip(1).First().Id,
                Booking = _dbContext.Bookings.Skip(1).First(),
                RoomId = _dbContext.Rooms.Skip(1).First().Id,
                Room = _dbContext.Rooms.Skip(1).First()
            };

            var bookingRoom3 = new BookingRoom
            {
                BookingId = _dbContext.Bookings.Skip(2).First().Id,
                Booking = _dbContext.Bookings.Skip(2).First(),
                RoomId = _dbContext.Rooms.Skip(2).First().Id,
                Room = _dbContext.Rooms.Skip(2).First()
            };

            var bookingRoom4 = new BookingRoom
            {
                BookingId = _dbContext.Bookings.Skip(3).First().Id,
                Booking = _dbContext.Bookings.Skip(3).First(),
                RoomId = _dbContext.Rooms.Skip(3).First().Id,
                Room = _dbContext.Rooms.Skip(3).First()
            };

            _dbContext.BookingRooms.AddRange(new[] { bookingRoom1, bookingRoom2, bookingRoom3, bookingRoom4 });
            _dbContext.SaveChanges();
        }



        private void LinkRelationships()
        {
            foreach (var bookingRoom in _dbContext.BookingRooms)
            {
                var room = _dbContext.Rooms.FirstOrDefault(r => r.Id == bookingRoom.RoomId);
                var booking = _dbContext.Bookings.FirstOrDefault(b => b.Id == bookingRoom.Id);

                if (room != null)
                {
                    room.ListOfBookingRoomsInRoom.Add(bookingRoom);
                }

                if (booking != null)
                {
                    booking.ListOfBookingRoomsInBooking.Add(bookingRoom);
                }
            }
            foreach (var booking in _dbContext.Bookings)
            {
                var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == booking.CustomerId);

                if (customer != null)
                {
                    customer.ListOfBookingsInCustomer.Add(booking);
                }
            }
            _dbContext.SaveChanges();
        }
    }
}
