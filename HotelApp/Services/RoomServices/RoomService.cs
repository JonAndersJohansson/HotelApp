using HotelApp.Data;
using HotelApp.Models;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services
{
    public class RoomService
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<RoomMenu> _roomMenu;
        private ApplicationDbContext_FAKE _dbContext;
        public RoomService(DisplayList displayList, Lazy<RoomMenu> roomMenu, ApplicationDbContext_FAKE dbContext)
        {
            _displayList = displayList;
            _roomMenu = roomMenu;
            _dbContext = dbContext;
        }
        public int GetARoomIndex()
        {
            var selectedIndex = _displayList.BrowseAList(_dbContext.Rooms, false, Graphics.GetHeaderAsString("Välj rum för att visa all info ↑/↓/↩"), false);
            if (selectedIndex >= 0 && selectedIndex < _dbContext.Rooms.Count)
                return selectedIndex;
            else
                return -1;
        }
        public Room GetARoom(int roomIndex)
        {
            var selectedRoom = _dbContext.Rooms[roomIndex];
            return selectedRoom;
        }
        public void ReadOneRoom(Room selectedRoom)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString($"  Info Rum: {selectedRoom.RoomNumberAsID}"));
            Console.ResetColor();

            // Visa all information om rummet
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  Rumssnummer: {selectedRoom.RoomNumberAsID}");
            Console.WriteLine($"  Rumstyp: {selectedRoom.RoomType}");
            Console.WriteLine($"  Antal extra sängar: {selectedRoom.NumberOfPossibleExtraBeds}");
            Console.WriteLine($"  Kostnad per natt: {selectedRoom.CostPerNight:C}");
            Console.WriteLine($"  Tillgängligt för funktionshindrade: {(selectedRoom.IsDisabilityFriendly ? "Ja" : "Nej")}");
            Console.WriteLine($"  Övrigt: {selectedRoom.OtherOrDescription}");
            Console.WriteLine($"  Aktivt: {(selectedRoom.IsActive ? "Ja" : "Nej")}");
            Console.ResetColor();
            // Visa relaterade bokningar (om några finns)
            if (selectedRoom.ListOfBookingRoomsInRoom?.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  --- Relaterade bokningar ---");
                foreach (var bookingRoom in selectedRoom.ListOfBookingRoomsInRoom)
                {
                    Console.WriteLine($"      Bokningnummer: {bookingRoom.BookingId}");
                    Console.WriteLine($"      Namn: {bookingRoom.Booking.CustomerInBooking.LastName} ,{bookingRoom.Booking.CustomerInBooking.FirstName}");
                    Console.WriteLine($"      Antal besökare: {bookingRoom.Booking.NumberOfGuests}");
                    Console.WriteLine($"      Incheckning: {bookingRoom.Booking.StartDate}");
                    Console.WriteLine($"      Utcheckning: {bookingRoom.Booking.EndDate}");
                    Console.WriteLine("      -");
                }
            }
            else
            {
                Console.WriteLine("\n  Inga relaterade bokningar.");
            }
            Console.ResetColor();


            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _roomMenu.Value.MenuSwitch();
        }

        public short GetRoomNumber()
        {
            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString($"Rumsnummer"));
                Console.ResetColor();
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Följ standard. Första numret anger våning, därefter löpande.\n   2. Det måste vara ett positivt nummer mellan 1 och 999.");
                Messages.SetValueWithCursor();

                string inputRoomNumber = Console.ReadLine();

                if (inputRoomNumber.ToLower() == "exit")
                    return -1;

                if (short.TryParse(inputRoomNumber, out short roomNumber))
                {
                    if (roomNumber > 0 && roomNumber < 999)
                        return roomNumber;
                    else
                        Console.WriteLine("\n  Rumsnumret måste vara mellan 1 och 999. Försök igen.");
                }
                else
                    Console.WriteLine("\n  Ogiltigt rumsnummer. Vänligen ange ett nummer.");

                Console.WriteLine("\n  Tryck valfri tangent för att försöka igen...");
                Console.ReadKey();
            }
        }
        public void AddRoom(Room room)
        {
            _dbContext.Rooms.Add(room);

            Console.Write("\n  Rum ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(room.RoomNumberAsID);
            Console.ResetColor();
            Console.WriteLine(" har sparats.");
            Thread.Sleep(1000);
            ReadOneRoom(room);
        }

        internal void DeactivateARoom(Room selectedRoom)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString("Avaktivera / Aktivera rum"));
            Console.ResetColor();

            if (selectedRoom.IsActive == true)
                selectedRoom.IsActive = false;
            else
                selectedRoom.IsActive = true;
            Console.Write("\n  Följande rum är ändrat: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(selectedRoom.RoomNumberAsID);
            Console.ResetColor();

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _roomMenu.Value.MenuSwitch();
        }
        public bool ValidateRoom(Room room, bool isNew)
        {
            if (room.RoomNumberAsID < 0 || room.RoomNumberAsID > 999)
                return false;
            if (room.CostPerNight <= 0)
                return false;
            if (_dbContext.Rooms.Any(r => r.RoomNumberAsID == room.RoomNumberAsID) && isNew == true)
            {
                Console.WriteLine("\n  Ett rum med detta nummer finns redan.\n  Tryck på valfri tangent för att försöka igen...");
                Console.ReadKey();
                return false;
            }

            return true;
        }

        public BedSize GetRoomType()
        {
            List<string> listOfBedSizes = new List<string>
            {
            "Enkelrum", "Dubbelrum"
            };

            var selectedBedSize = _displayList.BrowseAList(listOfBedSizes, false, Graphics.GetHeaderAsString("Välj typ av rum ↑/↓/↩"), false);
            if (selectedBedSize == 0)
                return BedSize.Single;
            else
                return BedSize.Double;
        }

        public byte GetNumberOfPossibleBeds()
        {
            List<byte> listOfNumbers = new List<byte>
            {
            0, 1, 2, 3, 4, 5, 6, 7, 8
            };
            return (byte)_displayList.BrowseAList(listOfNumbers, false, Graphics.GetHeaderAsString("Välj antal möjliga extrasängar ↑/↓/↩"), false);
        }

        public decimal GetCostPerNight()
        {
            decimal costPerNight;
            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString($"Rummets kostnad per natt"));
                Console.ResetColor();
                Messages.RequiredInputMessage();

                Console.WriteLine("   1. Beloppet måste vara en siffra mellan 0 och 100000");
                Messages.SetValueWithCursor();

                string costPerNightInput = Console.ReadLine();
                if (costPerNightInput.ToLower() == "exit")
                    return 0;

                if (decimal.TryParse(costPerNightInput, out costPerNight))
                {
                    if (costPerNight >= 0 && costPerNight <= 100000)
                        return costPerNight;
                    else
                        Console.WriteLine("\n  Värdet måste vara mellan 0 och 100000. Försök igen.");
                }
                else
                    Console.WriteLine("\n  Ogiltig inmatning. Ange ett giltigt värde.");
            }
        }

        public bool GetYesOrNo(bool isActive)
        {
            string messageActive = "Skall rummer vara aktivt? ↑/↓/↩";
            string messageToUse = "Är rummet handikappanpassat? ↑/↓/↩";
            if (isActive)
                messageToUse = messageActive;

            List<string> listOfChoices = new List<string>
            {
            "JA", "NEJ"
            };
            byte selectedIndex = (byte)_displayList.BrowseAList(listOfChoices, false, Graphics.GetHeaderAsString(messageToUse), false);
            if (selectedIndex == 0)
                return true;
            else
                return false;
        }

        public string? GetOtherOrDescription()
        {
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString($"Övriga uppgifter"));
            Console.ResetColor();

            Console.Write("  Ange övriga relevanta uppgifter om rummet (valfritt): ");
            return Console.ReadLine();
        }
    }
}
