using HotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Data
{
    public class ApplicationDbContext_FAKE
    {
        // Dessa motsvarar DbSet<T> i EF-Core
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public List<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
    }
}
