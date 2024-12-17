using HotelApp.Controllers;
using HotelApp.Data;
using HotelApp.Data.Models;
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
        private readonly Lazy<RoomController>_roomController;
        private ApplicationDbContext_FAKE _dbContext;
        public RoomService(DisplayList displayList, Lazy<RoomController> roomController, ApplicationDbContext_FAKE dbContext)
        {
            _displayList = displayList;
            _roomController = roomController;
            _dbContext = dbContext;
        }
        public int GetARoomIndex(bool isDeactivate, bool isToChange)
        {
            string messageToUseInHeader = "Välj rum för att visa all info ↑/↓/↩";
            if (isDeactivate)
                messageToUseInHeader = "Välj rum för att Avaktivera / Aktivera ↑/↓/↩";
            if (isToChange)
                messageToUseInHeader = "Välj rum att ändra ↑/↓/↩";
            var selectedIndex = _displayList.BrowseAList(_dbContext.Rooms, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < _dbContext.Rooms.Count)
                return selectedIndex;
            else if (selectedIndex == -1)
            {
                _roomController.Value.MenuSwitch();
                return -1;
            }
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
            Console.WriteLine(Graphics.GetHeaderAsString($"  Info Rum: {selectedRoom.RoomNumber}"));
            Console.ResetColor();

            // Visa all information om rummet
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  Rumssnummer: {selectedRoom.RoomNumber}");
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
                var today = DateTime.Now;

                var sortedBookings = selectedRoom.ListOfBookingRoomsInRoom
                    .OrderBy(b => b.Booking.StartDate)
                    .ToList();

                var beforeToday = sortedBookings.Where(b => b.Booking.StartDate < today).TakeLast(3).ToList();
                var afterToday = sortedBookings.Where(b => b.Booking.StartDate >= today).Take(3).ToList();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  --- Kommande bokningar (3 närmsta) ---");
                foreach (var bookingRoom in afterToday)
                {
                    Console.WriteLine($"      Bokningnummer: {bookingRoom.BookingId}");
                    Console.WriteLine($"      Namn: {bookingRoom.Booking.CustomerInBooking.LastName}, {bookingRoom.Booking.CustomerInBooking.FirstName}");
                    Console.WriteLine($"      Antal besökare: {bookingRoom.Booking.NumberOfGuests}");
                    Console.WriteLine($"      Incheckning: {bookingRoom.Booking.StartDate}");
                    Console.WriteLine($"      Utcheckning: {bookingRoom.Booking.EndDate}");
                    Console.WriteLine($"      Övrig info: {bookingRoom.Booking.OtherInfoInBooking}");
                    Console.WriteLine("      -");
                }
                Console.WriteLine("\n     --- Tidigare bokningar (3 senaste) ---");
                foreach (var bookingRoom in beforeToday)
                {
                    Console.WriteLine($"         Bokningnummer: {bookingRoom.BookingId}");
                    Console.WriteLine($"         Namn: {bookingRoom.Booking.CustomerInBooking.LastName}, {bookingRoom.Booking.CustomerInBooking.FirstName}");
                    Console.WriteLine($"         Antal besökare: {bookingRoom.Booking.NumberOfGuests}");
                    Console.WriteLine($"         Incheckning: {bookingRoom.Booking.StartDate}");
                    Console.WriteLine($"         Utcheckning: {bookingRoom.Booking.EndDate}");
                    Console.WriteLine($"         Övrig info: {bookingRoom.Booking.OtherInfoInBooking}");
                    Console.WriteLine("         -");
                }
            }
            else
            {
                Console.WriteLine("\n  Inga relaterade bokningar.");
            }
            Console.ResetColor();


            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _roomController.Value.MenuSwitch();
        }

        public Room GetRoomNumber(Room room, bool isNew)
        {
            string messageToUseInHeader = "Uppdatera rumsnummer";
            if (isNew)
                messageToUseInHeader = "Skapa nytt rumsnummer";

            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
                Console.ResetColor();
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Följ standard. Första numret anger våning, därefter löpande.\n   2. Det måste vara ett positivt heltal mellan 1 och 999.");
                if (!isNew)
                {
                    Console.Write("  Nuvarande rummsnummer: ");
                    Console.ForegroundColor= ConsoleColor.Magenta;
                    Console.WriteLine(room.RoomNumber);
                    Console.ResetColor();
                }
                Messages.SetValueWithCursor();

                string inputRoomNumber = Console.ReadLine();

                if (inputRoomNumber.ToLower() == "exit")
                    return room;

                if (short.TryParse(inputRoomNumber, out short roomNumber))
                {
                    if (roomNumber > 0 && roomNumber < 999)
                    {
                        if (!_dbContext.Rooms.Any(r => r.RoomNumber == roomNumber))
                        {
                            room.RoomNumber = roomNumber;
                            Messages.SuccessfullInput();
                            return room;
                        }
                        else
                            Console.WriteLine("\n  Rumsnumret är redan upptaget. Försök igen.");

                    }
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
            Console.Write(room.RoomNumber);
            Console.ResetColor();
            Console.WriteLine(" har sparats.");
            Thread.Sleep(1000);
            ReadOneRoom(room);
        }

        public void DeactivateARoom(Room selectedRoom)
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
            Console.WriteLine(selectedRoom.RoomNumber);
            Console.ResetColor();

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _roomController.Value.MenuSwitch();
        }
        public bool ValidateRoom(Room room, bool isNew)
        {
            if (room.RoomNumber < 0 || room.RoomNumber > 999)
                return false;
            if (room.CostPerNight <= 0)
                return false;
            if (_dbContext.Rooms.Any(r => r.RoomNumber == room.RoomNumber) && isNew == true)
            {
                Console.WriteLine("\n  Ett rum med detta nummer finns redan.\n  Tryck på valfri tangent för att försöka igen...");
                Console.ReadKey();
                return false;
            }

            return true;
        }

        public Room GetRoomType(Room room, bool isNew)
        {
            List<string> listOfBedSizes = new List<string>
            {
            "Enkelrum", "Dubbelrum"
            };
            string messageToUseInHeader = $"Välj typ av rum ↑/↓/↩ - Nuvarande värde: {(room.RoomType == BedSize.Single ? "Enkelrum" : "Dubbelrum")}";
            if (isNew)
                messageToUseInHeader = $"Välj typ av rum ↑/↓/↩ - Standardvärde: {(room.RoomType == BedSize.Single ? "Enkelrum" : "Dubbelrum")}";

            var selectedBedSize = _displayList.BrowseAList(listOfBedSizes, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);

            if (selectedBedSize == -1)
                return room;
            else if (selectedBedSize == 0)
            {
                room.RoomType = BedSize.Single;
                Messages.SuccessfullInput();
                return room;
            }
            else if (selectedBedSize == 1)
            {
                room.RoomType = BedSize.Double;
                Messages.SuccessfullInput();
                return room;
            }
            else
                return room;
        }

        public Room GetNumberOfPossibleBeds(Room room, bool isNew)
        {
            List<byte> listOfNumbers = new List<byte>
            {
            0, 1, 2, 3, 4, 5, 6, 7, 8
            };
            string messageToUseInHeader = $"Välj antal möjliga extrasängar ↑/↓/↩ - Nuvarande värde: {room.NumberOfPossibleExtraBeds}";
            if (isNew)
                messageToUseInHeader = $"Välj antal möjliga extrasängar ↑/↓/↩ - Standardvärde: {room.NumberOfPossibleExtraBeds}";

            var numberOfPossibleBedsInput = _displayList.BrowseAList(listOfNumbers, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (numberOfPossibleBedsInput == -1)
                return room;
            else if (numberOfPossibleBedsInput > -1 && numberOfPossibleBedsInput < 9)
            {
                room.NumberOfPossibleExtraBeds = (byte)numberOfPossibleBedsInput;
                Messages.SuccessfullInput();
                return room;
            } 
            else
                return room;
        }

        public Room GetCostPerNight(Room room, bool isNew)
        {
            string messageToUseInHeader = "Uppdatera rummets baskostnad per natt";
            if (isNew)
                messageToUseInHeader = "Baskostnad per natt";
            decimal costPerNight;
            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
                Console.ResetColor();
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Beloppet måste vara en siffra mellan 0 och 100000");

                if (!isNew)
                {
                    Console.Write($"  Nuvarande baskostnad: ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(room.CostPerNight);
                    Console.ResetColor();
                }
                Messages.SetValueWithCursor();

                string costPerNightInput = Console.ReadLine();
                if (costPerNightInput.ToLower() == "exit")
                    return room;

                if (decimal.TryParse(costPerNightInput, out costPerNight))
                {
                    if (costPerNight >= 0 && costPerNight <= 100000)
                    {
                        room.CostPerNight = costPerNight;
                        Messages.SuccessfullInput();
                        return room;
                    }
                    else
                        Console.WriteLine("\n  Värdet måste vara mellan 0 och 100000. Försök igen.");
                }
                else
                    Console.WriteLine("\n  Ogiltig inmatning. Ange ett giltigt värde.");
            }
        }

        public Room GetIsActive(Room room, bool isNew)
        {
            string messageToUseInHeader = $"Aktivera / Avaktivera rum. Nuvarande värde: {(room.IsActive ? "JA" : "NEJ")}";
            if (isNew)
                messageToUseInHeader = $"Aktivera / Avaktivera rum. Standardvärde: {(room.IsActive ? "JA" : "NEJ")}";
            
            List<string> listOfChoices = new List<string>
            {
            "JA", "NEJ"
            };
            var selectedIndex = _displayList.BrowseAList(listOfChoices, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex == -1)
                return room;
            else if (selectedIndex == 0)
            {
                room.IsActive = true;
                Messages.SuccessfullInput();
                return room;
            }
            else if (selectedIndex == 1)
            {
                room.IsActive = false;
                Messages.SuccessfullInput();
                return room;
            }
            else
                return room;
        }
        public Room GetIsDisabilityFriendly(Room room, bool isNew)
        {
            string messageToUseInHeader = $"Handikappanpassat rum. Nuvarande värde: {(room.IsDisabilityFriendly ? "JA" : "NEJ")}";
            if (isNew)
                messageToUseInHeader = $"Handikappanpassat rum. Standardvärde: {(room.IsDisabilityFriendly ? "JA" : "NEJ")}";
            List<string> listOfChoices = new List<string>
            {
            "JA", "NEJ"
            };
            var selectedIndex = _displayList.BrowseAList(listOfChoices, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex == -1)
                return room;
            else if (selectedIndex == 0)
            {
                room.IsDisabilityFriendly = true;
                Messages.SuccessfullInput();
                return room;
            }
            else if(selectedIndex == 1)
            {
                room.IsDisabilityFriendly = false;
                Messages.SuccessfullInput();
                return room;
            }
            else
                return room;
        }

        public Room GetOtherOrDescription(Room room, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera övriga uppgifter";
            if (isNew)
                messageToUseInHeader = $"Övriga uppgifter";

            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();
            if (!isNew)
                Console.WriteLine($"  Nuvarande uppgifter:\n  {room.OtherOrDescription}\n\n  Uppdatera övriga uppgifter (valfritt):");
            else
                Console.WriteLine("  Ange övriga uppgifter (valfritt): ");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor + 0);
            string? otherOrDescriptionInput = Console.ReadLine();
            room.OtherOrDescription = otherOrDescriptionInput;
            Messages.SuccessfullInput();
            return room;
        }
        public string FormatRoomCombination(List<Room> roomCombination)
        {
            if (roomCombination == null || !roomCombination.Any())
                return "  Inga rum i denna kombination";

            var roomDescriptions = roomCombination
                .Select(room => room.ToString()) // Använder ToString() för varje rum
                .ToList();
            return string.Join(" & ", roomDescriptions); // Kombinera rummen med " | "
        }

    }
}
