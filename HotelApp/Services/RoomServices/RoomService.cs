using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.Services.RoomServices;
using HotelApp.UI;
using HotelApp.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Services
{
    public class RoomService
    {
        private readonly DisplayList _displayList;
        private ApplicationDbContext _dbContext;
        private readonly Lazy<RoomPropertySelector> _roomPropertySelector;
        public RoomService(DisplayList displayList, ApplicationDbContext dbContext,
            Lazy<RoomPropertySelector> roomPropertySelector)
        {
            _displayList = displayList;
            _dbContext = dbContext;
            _roomPropertySelector = roomPropertySelector;
        }
        public void SelectIndexInRooms(bool isDeactivate, bool isToChange)
        {
            string messageToUseInHeader = "Välj rum för att visa all info ↑/↓/↩";
            if (isDeactivate)
                messageToUseInHeader = "Välj rum för att Avaktivera / Aktivera ↑/↓/↩";
            if (isToChange)
                messageToUseInHeader = "Välj rum att ändra ↑/↓/↩";

            var rooms = _dbContext.Rooms
                .Where(r => r.IsActive)
                .ToList();

            var selectedIndex = _displayList.BrowseAList(rooms, false, 
                Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < rooms.Count)
                SelectRoomByIndex(selectedIndex, isDeactivate, isToChange);
            else if (selectedIndex == -1)
                return;
            else
            {
                Console.WriteLine("  Fel: Ogiltigt värde i SelectIndexInRooms." +
                    "\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
            }
        }
        public void SelectRoomByIndex(int roomIndex, bool isDeactivate, 
            bool isToChange)
        {
            var rooms = _dbContext.Rooms
                .Where(r => r.IsActive)
                .ToList();
            var selectedRoom = rooms[roomIndex];

            if (isDeactivate && selectedRoom != null)
                DeactivateARoom(selectedRoom);
            else if (isToChange && selectedRoom != null)
                _roomPropertySelector.Value.PropertySwitch(selectedRoom, false);
            else if (!isDeactivate && !isToChange && selectedRoom != null)
                ReadOneRoom(selectedRoom);
            else
            {
                Console.WriteLine("  Fel: Ogiltigt värde SelectRoomByIndex." +
                    "\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
            }
        }
        public void ReadOneRoom(Room selectedRoom)
        {
            _dbContext.Entry(selectedRoom)
                .Collection(r => r.ListOfBookingRoomsInRoom)
                .Query()
                .Include(br => br.Booking)
                    .ThenInclude(b => b.CustomerInBooking)
                .Load();

            Messages.ClearAndShowHeader($"  Info Rum: {selectedRoom.RoomNumber}");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  Rumssnummer: {selectedRoom.RoomNumber}");
            Console.WriteLine($"  Rumstyp: {selectedRoom.RoomType}");
            Console.WriteLine($"  Antal extra sängar: " +
                $"{selectedRoom.NumberOfPossibleExtraBeds}");
            Console.WriteLine($"  Kostnad per natt: {selectedRoom.CostPerNight:C}");
            Console.WriteLine($"  Tillgängligt för funktionshindrade: " +
                $"{(selectedRoom.IsDisabilityFriendly ? "Ja" : "Nej")}");
            Console.WriteLine($"  Övrigt: {selectedRoom.OtherOrDescription}");
            Console.WriteLine($"  Aktivt: {(selectedRoom.IsActive ? "Ja" : "Nej")}");
            Console.ResetColor();

            if (selectedRoom.ListOfBookingRoomsInRoom?.Count > 0)
            {
                var today = DateTime.Now;

                var sortedBookings = selectedRoom.ListOfBookingRoomsInRoom
                    .OrderBy(b => b.Booking.StartDate)
                    .ToList();

                var beforeToday = sortedBookings
                    .Where(b => b.Booking.StartDate < today)
                    .TakeLast(3)
                    .ToList();
                var afterToday = sortedBookings
                    .Where(b => b.Booking.StartDate >= today)
                    .Take(3)
                    .ToList();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  --- Kommande bokningar (3 närmsta) ---");
                foreach (var bookingRoom in afterToday)
                {
                    Console.WriteLine($"      Bokningnummer: {bookingRoom.Id}");
                    Console.WriteLine($"      Namn: {bookingRoom.Booking.
                        CustomerInBooking.LastName}, {bookingRoom.Booking.
                        CustomerInBooking.FirstName}");
                    Console.WriteLine($"      Antal besökare: {bookingRoom.
                        Booking.NumberOfGuests}");
                    Console.WriteLine($"      Incheckning: {bookingRoom.
                        Booking.StartDate}");
                    Console.WriteLine($"      Utcheckning: {bookingRoom.
                        Booking.EndDate}");
                    Console.WriteLine($"      Övrig info: {bookingRoom.
                        Booking.OtherInfoInBooking}");
                    Console.WriteLine("      -");
                }
                Console.WriteLine("\n     --- Tidigare bokningar (3 senaste) ---");
                foreach (var bookingRoom in beforeToday)
                {
                    Console.WriteLine($"         Bokningnummer: {bookingRoom.Id}");
                    Console.WriteLine($"         Namn: {bookingRoom.Booking.
                        CustomerInBooking.LastName}, {bookingRoom.Booking.
                        CustomerInBooking.FirstName}");
                    Console.WriteLine($"         Antal besökare: {bookingRoom.
                        Booking.NumberOfGuests}");
                    Console.WriteLine($"         Incheckning: {bookingRoom.
                        Booking.StartDate}");
                    Console.WriteLine($"         Utcheckning: {bookingRoom.
                        Booking.EndDate}");
                    Console.WriteLine($"         Övrig info: {bookingRoom.
                        Booking.OtherInfoInBooking}");
                    Console.WriteLine("         -");
                }
            }
            else
                Console.WriteLine("\n  Inga relaterade bokningar.");
            Console.ResetColor();

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }
        public Room GetRoomNumber(Room room, bool isNew)
        {
            string messageToUseInHeader = "Uppdatera rumsnummer";
            if (isNew)
                messageToUseInHeader = "Skapa nytt rumsnummer";

            while (true)
            {
                Messages.ClearAndShowHeader(messageToUseInHeader);
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Följ standard. Första numret anger " +
                    "våning, därefter löpande.\n   2. Det måste vara ett " +
                    "positivt heltal mellan 1 och 999.");
                if (!isNew)
                {
                    Console.Write("  Nuvarande rummsnummer: ");
                    Console.ForegroundColor= ConsoleColor.Magenta;
                    Console.WriteLine(room.RoomNumber);
                    Console.ResetColor();
                }
                Messages.SetValueWithCursor();

                string? inputRoomNumber = Console.ReadLine();

                if (inputRoomNumber?.ToLower() == "exit")
                    return room;

                if (short.TryParse(inputRoomNumber, out short roomNumber))
                {
                    if (roomNumber > 0 && roomNumber < 999)
                    {
                        if (!_dbContext.Rooms.Any(r => r.RoomNumber == roomNumber))
                        {
                            room.RoomNumber = roomNumber;
                            Messages.SuccessfullInputSave();
                            return room;
                        }
                        else
                            Console.WriteLine("\n  Rumsnumret är redan upptaget." +
                                " Försök igen.");

                    }
                    else
                        Console.WriteLine("\n  Rumsnumret måste vara mellan 1 " +
                            "och 999. Försök igen.");
                }
                else
                    Console.WriteLine("\n  Ogiltigt rumsnummer. Vänligen ange " +
                        "ett nummer.");

                Console.WriteLine("\n  Tryck valfri tangent för att försöka " +
                    "igen...");
                Console.ReadKey();
            }
        }
        public void DeactivateARoom(Room selectedRoom)
        {
            Messages.ClearAndShowHeader("Avaktivera / Aktivera rum");

            if (!_dbContext.Rooms.Local.Any(r => r.Id == selectedRoom.Id))
            {
                _dbContext.Attach(selectedRoom);
            }
            selectedRoom.IsActive = !selectedRoom.IsActive;

            _dbContext.SaveChanges();

            Console.Write("\n  Följande rum är ändrat: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Rum {selectedRoom.RoomNumber} är nu " +
                $"{(selectedRoom.IsActive ? "Aktivt" : "Inaktivt")}.");
            Console.ResetColor();
            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }
        public Room GetRoomType(Room room, bool isNew)
        {
            List<string> listOfBedSizes = new List<string>
            {
            "Enkelrum", "Dubbelrum"
            };
            string messageToUseInHeader = $"Välj typ av rum ↑/↓/↩ - " +
                $"Nuvarande värde: {(room.RoomType == BedSize.Single ? 
                "Enkelrum" : "Dubbelrum")}";
            if (isNew)
                messageToUseInHeader = $"Välj typ av rum ↑/↓/↩ - Standardvärde: " +
                    $"{(room.RoomType == BedSize.Single ? "Enkelrum" : "Dubbelrum")}";

            var selectedBedSize = _displayList.BrowseAList(listOfBedSizes, 
                false, Graphics.GetHeaderAsString(messageToUseInHeader), false);

            if (selectedBedSize == -1)
                return room;
            else if (selectedBedSize == 0)
            {
                room.RoomType = BedSize.Single;
                Messages.SuccessfullInputSave();
                return room;
            }
            else if (selectedBedSize == 1)
            {
                room.RoomType = BedSize.Double;
                Messages.SuccessfullInputSave();
                return room;
            }
            else
                return room;
        }
        public Room GetNumberOfPossibleExtraBeds(Room room, bool isNew)
        {
            if (room.RoomType == BedSize.Single)
            {
                Messages.ClearAndShowHeader("Fel: Enkelrum kan inte ha extrasängar.");
                Console.WriteLine("\n  Ett enkelrum kan inte ha extrasängar.");
                Console.WriteLine("  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                return room;
            }
            List<string> listOfNumbers = new List<string>
            {
                "1st (Rummet är mindre än 25m²)", "2st (Rummet är större än 25m²)", 
            };
            string messageToUseInHeader = $"Välj antal möjliga extrasängar " +
                $"↑/↓/↩ - Nuvarande värde: {room.NumberOfPossibleExtraBeds}";
            if (isNew)
                messageToUseInHeader = $"Välj antal möjliga extrasängar " +
                    $"↑/↓/↩ - Standardvärde: {room.NumberOfPossibleExtraBeds}";

            var numberOfPossibleBedsInput = _displayList.BrowseAList
                (listOfNumbers, false, Graphics.GetHeaderAsString
                (messageToUseInHeader), false);
            if (numberOfPossibleBedsInput == -1)
                return room;
            else if (numberOfPossibleBedsInput > -1 && numberOfPossibleBedsInput == 0)
            {
                room.NumberOfPossibleExtraBeds = 1;
                Messages.SuccessfullInputSave();
                return room;
            }
            else if (numberOfPossibleBedsInput > -1 && numberOfPossibleBedsInput == 1)
            {
                room.NumberOfPossibleExtraBeds = 2;
                Messages.SuccessfullInputSave();
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
                Messages.ClearAndShowHeader(messageToUseInHeader);
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Beloppet måste vara en siffra mellan " +
                    "0 och 100000");

                if (!isNew)
                {
                    Console.Write($"  Nuvarande baskostnad: ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(room.CostPerNight);
                    Console.ResetColor();
                }
                Messages.SetValueWithCursor();

                string? costPerNightInput = Console.ReadLine();
                if (costPerNightInput.ToLower() == "exit")
                    return room;

                if (decimal.TryParse(costPerNightInput, out costPerNight))
                {
                    if (costPerNight >= 0 && costPerNight <= 100000)
                    {
                        room.CostPerNight = costPerNight;
                        Messages.SuccessfullInputSave();
                        return room;
                    }
                    else
                    {
                        Console.WriteLine("\n  Värdet måste vara mellan 0 och " +
                            "100000.\n  Tryck på valfri tangent för att försöka igen...");
                        Console.ReadKey();
                        return room;
                    }
                        
                }
                else
                {
                    Console.WriteLine("\n  Värdet måste vara mellan 0 och " +
                                "100000.\n  Tryck på valfri tangent för att försöka igen...");
                    Console.ReadKey();
                    return room;
                }
            }
        }
        public Room GetIsActive(Room room, bool isNew)
        {
            string messageToUseInHeader = $"Aktivera / Avaktivera rum. " +
                $"Nuvarande värde: {(room.IsActive ? "JA" : "NEJ")}";
            if (isNew)
                messageToUseInHeader = $"Aktivera / Avaktivera rum. " +
                    $"Standardvärde: {(room.IsActive ? "JA" : "NEJ")}";
            
            List<string> listOfChoices = new List<string>
            {
            "JA", "NEJ"
            };
            var selectedIndex = _displayList.BrowseAList(listOfChoices, 
                false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex == -1)
                return room;
            else if (selectedIndex == 0)
            {
                room.IsActive = true;
                Messages.SuccessfullInputSave();
                return room;
            }
            else if (selectedIndex == 1)
            {
                room.IsActive = false;
                Messages.SuccessfullInputSave();
                return room;
            }
            else
                return room;
        }
        public Room GetIsDisabilityFriendly(Room room, bool isNew)
        {
            string messageToUseInHeader = $"Handikappanpassat rum. " +
                $"Nuvarande värde: {(room.IsDisabilityFriendly ? "JA" : "NEJ")}";
            if (isNew)
                messageToUseInHeader = $"Handikappanpassat rum. " +
                    $"Standardvärde: {(room.IsDisabilityFriendly ? "JA" : "NEJ")}";
            List<string> listOfChoices = new List<string>
            {
            "JA", "NEJ"
            };
            var selectedIndex = _displayList.BrowseAList(listOfChoices, 
                false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex == -1)
                return room;
            else if (selectedIndex == 0)
            {
                room.IsDisabilityFriendly = true;
                Messages.SuccessfullInputSave();
                return room;
            }
            else if(selectedIndex == 1)
            {
                room.IsDisabilityFriendly = false;
                Messages.SuccessfullInputSave();
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

            Messages.ClearAndShowHeader(messageToUseInHeader);
            if (!isNew)
                Console.WriteLine($"  Nuvarande uppgifter:\n  " +
                    $"{room.OtherOrDescription}\n\n  Uppdatera övriga " +
                    $"uppgifter (valfritt):");
            else
                Console.WriteLine("  Ange övriga uppgifter (valfritt): ");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor + 0);
            string? otherOrDescriptionInput = Console.ReadLine();
            room.OtherOrDescription = otherOrDescriptionInput;
            Messages.SuccessfullInputSave();
            return room;
        }
        public string FormatRoomCombination(List<Room> roomCombination)
        {
            if (roomCombination == null || !roomCombination.Any())
                return "  Inga rum i denna kombination";

            var roomDescriptions = roomCombination
                .Select(room => room.ToString())
                .ToList();
            return string.Join("\n        & ", roomDescriptions);
        }
        public bool ValidateRoom(Room room, bool isNew)
        {
            if (room.RoomNumber < 0 || room.RoomNumber > 999)
                return false;
            if (room.CostPerNight <= 0)
                return false;
            if (_dbContext.Rooms.Any(r => r.RoomNumber == room.RoomNumber) 
                && isNew == true)
            {
                Console.WriteLine("\n  Ett rum med detta nummer finns redan." +
                    "\n  Tryck på valfri tangent för att försöka igen...");
                Console.ReadKey();
                return false;
            }
            if (room.RoomType == BedSize.Single && room.NumberOfPossibleExtraBeds > 0)
            {
                Console.WriteLine("\n  Ett enkelrum kan inte ha extrasängar." +
                    "\n  Tryck på valfri tangent för att försöka igen...");
                Console.ReadKey();
                return false;
            }
            return true;
        }
        public void AddRoom(Room room)
        {
            var entry = _dbContext.Entry(room);

            if (entry.State == EntityState.Detached)  
                _dbContext.Rooms.Add(room);

            _dbContext.SaveChanges();

            Console.Write("\n  Rum ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(room.RoomNumber);
            Console.ResetColor();
            Console.WriteLine(" har sparats.");
            Thread.Sleep(1000);

            ReadOneRoom(room);
        }
    }
}
