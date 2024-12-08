using HotelApp.Models;
using HotelApp.UI;
using HotelApp.UX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.UI.Menus
{
    /// <summary>
    /// Klassen hanterar undermenyn RoomMenu
    /// </summary>
    public class RoomMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;

        public RoomMenu(Lazy<ServiceMenu> serviceMenu, DisplayList displayList)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
        }

        public readonly List<string> listRoomMenu = new List<string>
        {
        "Visa alla rum (Aktiva och icke aktiva)", "Lägg till ett nytt rum", "Ändra ett befintligt rum", "Avaktivera/aktivera ett rum"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
            var jon = new Customer
            {
                CustomerId = 53234,
                FirstName = "Jon",
                LastName = "Johansson",
                Address = "Fasanvägen 8, 89242 Domsjö",
                EmailAddress = "Jonjoahss@gmail.com",
                Membership = TypeOfMembership.Silver,
                PhoneNumber = "070628362"
            };

            {
            };

            var roomBookings = new List<Booking> { booking1 };

            var exempelRum = new List<Room>
            {
                new Room { RoomNumber = 103, RoomType = BedSize.Single, CostPerNight = 1670, IsActive = true, MaxPersonsAllowedInRoom = 3, NumberOfPossibleExtraBeds = 0, RoomSizeInSquareMeters = 19.2f, IsCurrentlyOccupied = true, ListOfBookingInRoom = roomBookings },
                new Room { RoomNumber = 204, RoomType = BedSize.Double, CostPerNight = 2400, IsActive = true, MaxPersonsAllowedInRoom = 4, NumberOfPossibleExtraBeds = 1, RoomSizeInSquareMeters = 25.5f, IsCurrentlyOccupied = false, ListOfBookingInRoom = null },
                new Room { RoomNumber = 114, RoomType = BedSize.Double, CostPerNight = 2160, IsActive = false, MaxPersonsAllowedInRoom = 2, NumberOfPossibleExtraBeds = 0, RoomSizeInSquareMeters = 20.0f, IsCurrentlyOccupied = false, ListOfBookingInRoom = new List<Booking>() },
                new Room { RoomNumber = 122, RoomType = BedSize.Single, CostPerNight = 1670, IsActive = true, MaxPersonsAllowedInRoom = 1, NumberOfPossibleExtraBeds = 0, RoomSizeInSquareMeters = 15.0f, IsCurrentlyOccupied = true, ListOfBookingInRoom = roomBookings },
                new Room { RoomNumber = 313, RoomType = BedSize.Single, CostPerNight = 3200, IsActive = true, MaxPersonsAllowedInRoom = 6, NumberOfPossibleExtraBeds = 2, RoomSizeInSquareMeters = 35.0f, IsCurrentlyOccupied = false, ListOfBookingInRoom = null },
                new Room { RoomNumber = 401, RoomType = BedSize.Double, CostPerNight = 4500, IsActive = true, MaxPersonsAllowedInRoom = 2, NumberOfPossibleExtraBeds = 0, RoomSizeInSquareMeters = 40.0f, IsCurrentlyOccupied = false, ListOfBookingInRoom = new List<Booking>() },
                };





            switch (_displayList.BrowseAList(listRoomMenu, false, Graphics.MakeHeader("Rum ↑/↓"), true))
            {
                case 0:
                    
                    Room selectedRoom = exempelRum[selectedIndex];

                    Console.Clear();
                    Console.WriteLine("Information om valt rum:");
                    DisplayRoomDetails(selectedRoom);

                    Console.WriteLine("\nTryck valfri tangent för att ta bort rummet...");
                    Console.ReadKey();

                    exempelRum.RemoveAt(selectedIndex);
                    Console.ReadKey();
                    break;
                case 1:
                    //Lägg till ett nytt rum
                    break;
                case 2:
                    //Uppdatera ett befintligt rum
                    break;
                case 3:
                    //Avaktivera/aktivera ett rum
                    break;
                case 4:
                    _serviceMenu.Value.MenuSwitch();
                    return;
                default:
                    Console.WriteLine("Ogiltigt alternativ 'RoomMenu', tryck valfri tangent för att återgå.");
                    Console.ReadKey();
                    _serviceMenu.Value.MenuSwitch();
                    break;
            }
        }
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
