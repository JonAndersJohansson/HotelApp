using HotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services.RoomServices
{
    public class ReadRoom
    {
        private void DisplayRoomDetails(Room room)
        {
            Console.WriteLine($"Rum Nummer: {room.RoomNumber}");
            Console.WriteLine($"Rumstyp: {room.RoomType}");
            Console.WriteLine($"Kostnad per natt: {room.CostPerNight} kr");
            Console.WriteLine($"Aktivt: {(room.IsActive ? "Ja" : "Nej")}");
            Console.WriteLine($"Max personer: {room.MaxPersonsAllowedInRoom}");
            Console.WriteLine($"Antal möjliga extrasängar: {room.NumberOfPossibleExtraBeds}");
            Console.WriteLine($"Storlek: {room.RoomSizeInSquareMeters} m²");
            Console.WriteLine($"Upptaget: {(room.IsCurrentlyOccupied ? "Ja" : "Nej")}");

            if (room.ListOfBookingInRoom != null && room.ListOfBookingInRoom.Any())
            {
                Console.WriteLine("Bokningar:");
                foreach (var booking in room.ListOfBookingInRoom)
                {
                    Console.WriteLine($"  - Boknings-ID: {booking.BookingId}, Kund: {booking.CustomerInBooking.FirstName} {booking.CustomerInBooking.LastName}");
                }
            }
            else
            {
                Console.WriteLine("Inga bokningar för detta rum.");
            }
        }

    }
}
