using HotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services.BookingService
{
    public enum BookingSearchProperty
    {
        BookingId,
        CustomerId,
        RoomNumber,
        StartDate,
        EndDate,
        IsCancelled
    }
    public class BookingService
    {
        public BookingSearchProperty GetSearchPropertyInBooking()
        {
            var properties = Enum.GetValues(typeof(BookingSearchProperty)).Cast<BookingSearchProperty>().ToList();

            Console.WriteLine("Välj vilken egenskap du vill söka efter:");
            for (int i = 0; i < properties.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {properties[i]}");
            }

            int selectedIndex;
            while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 1 || selectedIndex > properties.Count)
            {
                Console.WriteLine("Ogiltigt val, försök igen.");
            }

            return properties[selectedIndex - 1];
        }
        public Booking SearchBooking(BookingSearchProperty searchProperty, List<Booking> bookings)
        {
            switch (searchProperty)
            {
                case BookingSearchProperty.BookingId:
                    Console.Write("Ange bokningsnummer: ");
                    int bookingId = int.Parse(Console.ReadLine());
                    return bookings.FirstOrDefault(b => b.BookingId == bookingId);

                case BookingSearchProperty.CustomerId:
                    Console.Write("Ange kund-ID: ");
                    int customerId = int.Parse(Console.ReadLine());
                    return bookings.FirstOrDefault(b => b.CustomerInBooking.CustomerId == customerId);

                case BookingSearchProperty.RoomNumber:
                    Console.Write("Ange rum-ID: ");
                    int roomId = int.Parse(Console.ReadLine());
                    return bookings.FirstOrDefault(b => b.RoomNumber == roomId);

                case BookingSearchProperty.StartDate:
                    Console.Write("Ange startdatum (yyyy-mm-dd): ");
                    DateTime startDate = DateTime.Parse(Console.ReadLine());
                    return bookings.FirstOrDefault(b => b.StartDate.Date == startDate.Date);

                case BookingSearchProperty.EndDate:
                    Console.Write("Ange slutdatum (yyyy-mm-dd): ");
                    DateTime endDate = DateTime.Parse(Console.ReadLine());
                    return bookings.FirstOrDefault(b => b.EndDate.Date == endDate.Date);

                case BookingSearchProperty.IsCancelled:
                    Console.Write("Är avbokad? (yes/no): ");
                    bool isCancelled = Console.ReadLine()?.ToLower() == "yes";
                    return bookings.FirstOrDefault(b => b.IsCancelled == isCancelled);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void SearchMenu(List<Booking> bookings)
        {
            var searchProperty = GetSearchPropertyInBooking();
            var booking = SearchBooking(searchProperty, bookings);

            if (booking != null)
            {
                Console.WriteLine($"Bokning hittad: {booking}");
            }
            else
            {
                Console.WriteLine("Ingen bokning hittades med de angivna kriterierna.");
            }
        }
    }
}

