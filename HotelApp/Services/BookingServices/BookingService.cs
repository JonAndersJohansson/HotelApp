using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.UI;
using HotelApp.Utilities;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Data;
using Calendar = HotelApp.Utilities.Calendar;

namespace HotelApp.Services.BookingService
{
    public class BookingService
    {
        private readonly DisplayList _displayList;
        private readonly ApplicationDbContext _dbContext;
        private readonly Lazy<RoomService> _roomService;
        private readonly Lazy<CustomerService> _customerService;
        private readonly Lazy<InvoiceService> _invoiceService;
        private readonly Lazy<BookingPropertySelector> _bookingPropertySelector;
        public BookingService(DisplayList displayList, ApplicationDbContext dbContext, Lazy<RoomService> roomService, Lazy<CustomerService> customerServices, Lazy<InvoiceService> invoiceService, Lazy<BookingPropertySelector> bookingPropertySelector)
        {
            _displayList = displayList;
            _dbContext = dbContext;
            _roomService = roomService;
            _customerService = customerServices;
            _invoiceService = invoiceService;
            _bookingPropertySelector = bookingPropertySelector;
        }
        public void CheckAvailability()
        {
            DateTime startDate;
            DateTime endDate;
            byte numberOfGuests;

            while (true)
            {
                startDate = GetDateByCalendar("Välj INCHECKNING");
                if (startDate == DateTime.MinValue)
                {
                    Messages.AbortBooking();
                    return;
                }
                if (startDate < DateTime.Now.Date)
                {
                    Console.WriteLine("  Ogiltig incheckningsdatum. Datumet kan inte vara bakåt i tiden.\n  Tryck på valfri tangent för att försöka igen...");
                    Console.ReadKey();
                    continue;
                }
                Messages.SuccessfullInput();

                endDate = GetDateByCalendar("Välj UTCHECKNING");
                if (endDate == DateTime.MinValue)
                {
                    Messages.AbortBooking();
                    return;
                }
                if (endDate < startDate)
                {
                    Console.WriteLine("  Ogiltig utcheckningsdatum. Datumet måste vara efter inckeckningsdatum.\n  Tryck på valfri tangent för att försöka igen...");
                    Console.ReadKey();
                    continue;
                }
                Messages.SuccessfullInput();
                break;
            }

            numberOfGuests = GetNumberOfGuests();
            if (numberOfGuests == 0)
            {
                Messages.AbortBooking();
                return;
            }
            Messages.SuccessfullInput();

            var selectedRooms = GetRoomsInBooking(startDate, endDate, numberOfGuests);
            if (selectedRooms == null)
            {
                Messages.AbortBooking();
                return;
            }
            Messages.SuccessfullInput();

            var otherInfo = GetOtherInfoAsString();
            if (otherInfo.ToLower() == "exit")
            {
                Messages.AbortBooking();
                return;
            }
            Messages.SuccessfullInput();

            if (selectedRooms.Any())
            {
                StartNewBooking(selectedRooms, startDate, endDate, numberOfGuests, otherInfo);
            }
            else
            {
                Console.WriteLine("\n  Ingen rumskombination vald, avbryter bokning.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                return;
            }
        }
        private List<Room>? GetRoomsInBooking(DateTime startDate, DateTime endDate, byte numberOfGuests)
        {
            List<Room> selectedRooms = new List<Room>();
            List<List<Room>>? listOfAvailableRoomCombinations = GetAvailableRooms(startDate, endDate, numberOfGuests);

            if (listOfAvailableRoomCombinations == null)
                return null;

            List<string> listOfFormattedRoomCombinations = listOfAvailableRoomCombinations
                .Select(combination => _roomService.Value.FormatRoomCombination(combination))
                .ToList();

            int selectedIndexInListOfFormattedRoomCompinations = _displayList.BrowseAList(listOfFormattedRoomCombinations, false, Graphics.GetHeaderAsString("Sökresultat lediga rumskombinationer"), false);

            if (selectedIndexInListOfFormattedRoomCompinations == -1)
                return null;
            else if (selectedIndexInListOfFormattedRoomCompinations >= listOfAvailableRoomCombinations.Count)
            {
                Console.WriteLine("  Ogiltigt värde. Avbryter bokning.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                return null;
            }
            return selectedRooms = listOfAvailableRoomCombinations[selectedIndexInListOfFormattedRoomCompinations];
        }

        public string? GetOtherInfoAsString()
        {
            Messages.ClearAndShowHeader("Övrig information om bokningen");
            Console.WriteLine("  Ange övrig information om bokningen (valfritt):");
            Console.Write("  Lämna fältet tomt och tryck ENTER om du inte vill ange något.\n  Skriv 'exit' om du vill avbryta bokningen");

            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor + 1);
            string? otherInfo = Console.ReadLine();

            return otherInfo;
        }

        public void StartNewBooking(List<Room> selectedRooms, DateTime startDate, DateTime endDate, byte numberOfGuests, string? otherInfo)
        {
            var customerInNewBooking = _customerService.Value.GetCustomer();
            if (customerInNewBooking == null)
            {
                Messages.AbortBooking();
                return;
            }
            else if (customerInNewBooking != null)
            {
                Messages.SuccessfullInput();
                CreateNewBooking(selectedRooms, startDate, endDate, numberOfGuests, customerInNewBooking, otherInfo);
            }
        }

        public void CreateNewBooking(List<Room> selectedRooms, DateTime startDate, DateTime endDate, byte numberOfGuests, Customer customer, string? otherInfo)
        {
            var newBooking = GenerateBookingFromInput(selectedRooms, startDate, endDate, numberOfGuests, customer, otherInfo);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Kund kopplad...");
            Console.ResetColor();
            Thread.Sleep(1000);

            SaveBookingToDataBase(newBooking);
            _invoiceService.Value.GenerateInvoiceOfBooking(newBooking);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Faktura skapad...");
            Console.ResetColor();
            Thread.Sleep(1000);

            ReadOneBooking(newBooking, true, false);
        }

        public void ReadOneBooking(Booking booking, bool isNewFromStart, bool isChanged)
        {
            string messageToUseInHeader = $"Visar bokningsnummer {booking.Id}";
            if (isNewFromStart)
                messageToUseInHeader = $"Bokning med bokningsnummer {booking.Id} skapad";
            if (isChanged)
                messageToUseInHeader = $"Bokning med bokningsnummer {booking.Id} ändrad";

            _dbContext.Entry(booking)
                .Reference(b => b.CustomerInBooking)
                .Load();
            _dbContext.Entry(booking)
                .Collection(b => b.ListOfBookingRoomsInBooking)
                .Query()
                .Include(br => br.Room)
                .Load();

            Messages.ClearAndShowHeader(messageToUseInHeader);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  BokningsNr: {booking.Id}");
            Console.WriteLine($"  Incheckning: {booking.StartDate:yyyy-MM-dd}");
            Console.WriteLine($"  Utcheckning: {booking.EndDate:yyyy-MM-dd}");
            Console.WriteLine($"  Antal gäster: {booking.NumberOfGuests}");
            Console.WriteLine($"  Annulerad: {(booking.IsCancelled ? "Ja" : "Nej")}");
            Console.WriteLine($"  Övrig information: {booking.OtherInfoInBooking ?? "Ingen"}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n     --- Kundinformation ---");
            Console.WriteLine($"     KundNr: {booking.CustomerInBooking.Id}");
            Console.WriteLine($"     Namn: {booking.CustomerInBooking.FirstName} {booking.CustomerInBooking.LastName}");
            Console.WriteLine($"     Telefonnummer: {booking.CustomerInBooking.PhoneNumber}");
            Console.WriteLine($"     Email: {booking.CustomerInBooking.EmailAddress}");
            Console.ResetColor();

            if (!isNewFromStart)
            {
                if (booking.ListOfBookingRoomsInBooking != null && booking.ListOfBookingRoomsInBooking.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\n        --- Kopplade Rum ---");
                    foreach (var bookingRoom in booking.ListOfBookingRoomsInBooking)
                    {
                        Console.WriteLine($"        RumNr: {bookingRoom.Room.RoomNumber}, Typ: {bookingRoom.Room.RoomType}, Pris per natt: {bookingRoom.Room.CostPerNight:C}");
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  Inga kopplade rum till denna bokning.");
                    Console.ResetColor();
                }

                if (booking.InvoiceInBooking != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n           --- Relaterad Faktura ---");
                    Console.WriteLine($"           FakturaNr: {booking.InvoiceInBooking.Id}, Belopp: {booking.InvoiceInBooking.TotalAmount:C}, Betald: {(booking.InvoiceInBooking.IsPaid ? "Ja" : "Nej")}, Förfallen: {(booking.InvoiceInBooking.IsOverDue ? "Ja" : "Nej")}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  Ingen relaterad faktura till denna bokning.");
                    Console.ResetColor();
                }
                Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
                Console.ReadKey();
            }
            if (isNewFromStart)
            {
                Console.WriteLine("\n  Kund skapad.\n  Tryck på valfri tangent för att gå tillbaka till bokningen...");
                Console.ReadKey();
            }
        }
        public void SaveBookingToDataBase(Booking newBooking)
        {
            _dbContext.Bookings.Add(newBooking);
            _dbContext.SaveChanges();
        }

        public Booking GenerateBookingFromInput(List<Room> selectedRooms, DateTime startDate, DateTime endDate, byte numberOfGuests, Customer customer, string? otherInfo)
        {
            if (selectedRooms == null || !selectedRooms.Any())
            {
                throw new ArgumentException("  Inga rum valdes. Bokning kan inte genereras.");
            }
            if (numberOfGuests <= 0)
            {
                throw new ArgumentException("  Antalet gäster måste vara större än 0.");
            }
            if (startDate >= endDate)
            {
                throw new ArgumentException("  Utcheckningsdatum måste vara senare än incheckningsdatum.");
            }

            var newBooking = new Booking
            {
                StartDate = startDate,
                EndDate = endDate,
                NumberOfGuests = numberOfGuests,
                CustomerInBooking = customer,
                CustomerId = customer.Id,
                OtherInfoInBooking = otherInfo,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            foreach (var room in selectedRooms)
            {
                newBooking.ListOfBookingRoomsInBooking.Add(new BookingRoom
                {
                    Booking = newBooking,
                    BookingId = newBooking.Id,
                    Room = room,
                    RoomId = room.Id,
                });
            }
            return newBooking;
        }
        public List<List<Room>>? GetAvailableRooms(DateTime startDate, DateTime endDate, byte numberOfGuests)
        {
            var activeRooms = _dbContext.Rooms
                .Where(r => r.IsActive)
                .ToList();

            var availableRooms = activeRooms
                .Where(r => IsRoomAvailable(r, startDate, endDate))
                .ToList();

            var roomCombinations = new List<List<Room>>();

            FindRoomCombinations(availableRooms, numberOfGuests, new List<Room>(), roomCombinations);

            if (roomCombinations == null || !roomCombinations.Any())
            {
                Console.WriteLine("  Inga lediga rumskombinationer hittades för det angivna datumet och antalet gäster.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                return null;
            }
            return roomCombinations;
        }

        bool IsRoomAvailable(Room room, DateTime startDate, DateTime endDate)
        {
            return !_dbContext.Bookings
                .Where(b => !b.IsCancelled)
                .Include(b => b.ListOfBookingRoomsInBooking)
                .Any(b => b.ListOfBookingRoomsInBooking.Any(br => br.RoomId == room.Id) &&
                          b.StartDate < endDate &&
                          b.EndDate > startDate);
        }
        private void FindRoomCombinations(List<Room> rooms, int remainingGuests, List<Room> currentCombination, List<List<Room>> result)
        {
            if (remainingGuests <= 0)
            {
                result.Add(new List<Room>(currentCombination));
                return;
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                int roomCapacity = GetRoomCapacity(room);

                if (roomCapacity >= remainingGuests || roomCapacity > 0)
                {
                    currentCombination.Add(room);

                    FindRoomCombinations(rooms.Skip(i + 1).ToList(), remainingGuests - roomCapacity, currentCombination, result);

                    currentCombination.RemoveAt(currentCombination.Count - 1);
                }
            }
        }
        private int GetRoomCapacity(Room room)
        {
            int baseCapacity = room.RoomType == BedSize.Double ? 2 : 1;
            return baseCapacity + room.NumberOfPossibleExtraBeds;
        }

        public byte GetNumberOfGuests()
        {
            Messages.ClearAndShowHeader("Antal gäster");
            Messages.RequiredInputMessage();
            Console.Write("  1. Värdet måste vara mellan 1-255.\n");
            Messages.SetValueWithCursor();

            byte numberOfGuests = 0;
            while (true)
            {
                string? input = Console.ReadLine();
                if (input.ToLower() == "exit")
                    return 0;

                if (string.IsNullOrWhiteSpace(input))
                    return 0;

                if (byte.TryParse(input, out numberOfGuests))
                {
                    if (numberOfGuests > 0)
                        break;

                    Console.WriteLine("  Antalet gäster måste vara större än 0.\n  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("  Ogiltig inmatning. Ange ett heltal mellan 1 och 255.\n  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
            }
            return numberOfGuests;
        }
        public DateTime GetDateByCalendar(string headerMessage)
        {
            var selectedStartDate = Calendar.GetDateTimeByCalendar(headerMessage);
            return selectedStartDate;
        }

        public void SearchBookingToList(bool isCancel, bool isToChange)
        {
            string messageToUseInHeader = "Sök bokning";
            if (isCancel)
                messageToUseInHeader = "Sök för att avboka en bokning";
            if (isToChange)
                messageToUseInHeader = "Sök för att ändra en bokning";

            Messages.ClearAndShowHeader(messageToUseInHeader);
            Messages.RequiredInputMessage();
            Console.WriteLine("   1. Sökbar info: Namn, BokningsNr, Incheckningsdatum/Utcheckningsdatum (YYYY-MM-DD)\n  Sök:");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(7, currentLineCursor - 1);
            string? userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("  Inga bokningar hittades som matchar din sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
            if (userInput.ToLower() == "exit")
            {
                return;
            }

            DateTime? parsedDate = null;
            if (DateTime.TryParse(userInput, out DateTime inputDate))
            {
                parsedDate = inputDate;
            }

            var matchingBookings = _dbContext.Bookings
                .Include(b => b.CustomerInBooking)
                .Include(b => b.ListOfBookingRoomsInBooking)
                    .ThenInclude(br => br.Room)
                .Where(b =>
                    EF.Functions.Like(b.CustomerInBooking.FirstName, $"%{userInput}%") ||
                    EF.Functions.Like(b.CustomerInBooking.LastName, $"%{userInput}%") ||
                    b.Id.ToString().Contains(userInput) ||
                    (parsedDate.HasValue &&
                    (b.StartDate.Date == parsedDate.Value || b.EndDate.Date == parsedDate.Value)))
                .ToList();


            if (!matchingBookings.Any())
            {
                Console.WriteLine("  Inga bokningar hittades som matchar din sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
            else
                SelectBookingInList(matchingBookings, isCancel, isToChange);
        }

        public void SelectBookingInList(List<Booking> matchingBookings, bool isToCancel, bool isToChange)
        {
            string messageToUseInHeader = "Sökresultat, välj bokning för att visa all info ↑/↓/↩";
            if (isToCancel)
                messageToUseInHeader = "Sökresultat, välj bokning för att AVBOKA ↑/↓/↩";
            if (isToChange)
                messageToUseInHeader = "Sökresultat, välj bokning för att ändra ↑/↓/↩";

            var selectedIndex = _displayList.BrowseAList(matchingBookings, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < matchingBookings.Count && isToCancel && !isToChange)
                CancelBooking(matchingBookings[selectedIndex]);
            else if (selectedIndex >= 0 && selectedIndex < matchingBookings.Count && !isToCancel && isToChange)
                _bookingPropertySelector.Value.PropertySwitch(matchingBookings[selectedIndex]);
            else if (selectedIndex >= 0 && selectedIndex < matchingBookings.Count && !isToCancel && !isToChange)
                ReadOneBooking(matchingBookings[selectedIndex], false, false);
            else if (selectedIndex == -1)
                return;
            else
            {
                Console.WriteLine("  Fel: Ogiltigt värde i SelectBookingInList.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
        }
        public void Get100ByStartDate(bool isPastStartDate)
        {
            List<Booking> top100Bookings;

            if (isPastStartDate)
            {
                top100Bookings = _dbContext.Bookings
                    .Where(b => b.StartDate < DateTime.Now)
                    .OrderByDescending(b => b.StartDate)
                    .Take(100)
                    .Include(b => b.CustomerInBooking)
                    .ToList();
            }
            else
            {
                top100Bookings = _dbContext.Bookings
                    .Where(b => b.StartDate >= DateTime.Now)
                    .OrderBy(b => b.StartDate)
                    .Take(100)
                    .Include(b => b.CustomerInBooking)
                    .ToList();
            }
            var selectedIndex = _displayList.BrowseAList(top100Bookings,
                false, Graphics.GetHeaderAsString("Visar 100 senaste BETALDA" +
                    " fakturorna. Välj en för att visa all info ↑/↓/↩"), false);
            if (selectedIndex >= 0 && selectedIndex < top100Bookings.Count)
                ReadOneBooking(top100Bookings[selectedIndex], false, false);
            else if (selectedIndex < -1 || selectedIndex > top100Bookings.Count)
            {
                Console.WriteLine("  Fel: Index kunde inte hittas i " +
                    "GetAInvoiceFrom100IsPaid.\n  Tryck valfri tangent för " +
                    "att fortsätta...");
                Console.ReadKey();
            }
        }
        public void CancelBooking(Booking booking)
        {
            if (booking == null)
            {
                Console.WriteLine("  Bokningen kan inte vara null. Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
                return;
            }
            if (booking.IsCancelled == true)
            {
                Console.WriteLine("  Bokningen är redan avbokad. Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
                return;
            }

            booking.IsCancelled = true;

            if (booking.InvoiceInBooking != null)
            {
                booking.InvoiceInBooking.IsCancelled = true;
            }

            var entry = _dbContext.Entry(booking);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Bookings.Attach(booking);
            }

            _dbContext.SaveChanges();

            Console.WriteLine($"\n  Bokningen med ID {booking.Id} och dess kopplade faktura har annullerats.\n  Tryck på valfri tangent för att återgå...");
            Console.ReadKey();
        }

        public void SearchCurrentVisitors()
        {
            DateTime today = DateTime.Now.Date;

            var currentBookings = _dbContext.Bookings
                .Include(b => b.ListOfBookingRoomsInBooking)
                    .ThenInclude(br => br.Room)
                .Include(b => b.CustomerInBooking)
                .Where(b => b.StartDate <= today && b.EndDate >= today && !b.IsCancelled)
                .ToList();

            if (!currentBookings.Any())
            {
                Console.WriteLine("\n  Inga gäster bor på hotellet just nu :(");
                Console.ReadKey();
                return;
            }
            ReadCurrentVisitors(currentBookings);
        }
        private void ReadCurrentVisitors(List<Booking> currentBookings)
        {
            Messages.ClearAndShowHeader("Nuvarande besökare");
            int visitors = 0;
            foreach (var booking in currentBookings)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  Kund: {booking.CustomerInBooking.FirstName} {booking.CustomerInBooking.LastName}, Antal gäster: {booking.NumberOfGuests}, INcheck: {booking.StartDate:yyyy-MM-dd}, UTcheck: {booking.EndDate:yyyy-MM-dd}");

                if (booking.ListOfBookingRoomsInBooking != null && booking.ListOfBookingRoomsInBooking.Any())
                {
                    Console.Write("  Rum: ");
                    foreach (var bookingRoom in booking.ListOfBookingRoomsInBooking)
                    {
                        var room = bookingRoom.Room;
                        Console.Write($"{room.RoomNumber} ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine("\n  -");
                visitors += (int)booking.NumberOfGuests;
            }
            Console.Write("  Totalt antal gäster: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(visitors);
            Console.ResetColor();
            Console.WriteLine("\n  Tryck på valfri tangent för att återgå till menyn...");
            Console.ReadKey();
        }
        public Booking ChangeDateByCalendar(Booking booking, string headerMessage, bool isStartDate)
        {
            if (isStartDate && booking.StartDate < DateTime.Now)
            {
                Console.WriteLine("\n  Bokningen är redan påbörjad så incheckningsdatum kan inte ändras.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                return booking;
            }
            var currentdate = booking.StartDate.Date.ToString();
            if (!isStartDate)
                currentdate = booking.EndDate.Date.ToString();
            var headerMessageWithCurrentDate = $"{headerMessage}. Nuvarande datum: {currentdate}";
            var selectedStartDate = Calendar.GetDateTimeByCalendar(headerMessageWithCurrentDate);
            if (selectedStartDate == DateTime.MinValue)
                return booking;
            if (isStartDate)
            {
                booking.StartDate = selectedStartDate;
                Messages.SuccessfullInputSave();
            }
            else if (!isStartDate)
            {
                booking.EndDate = selectedStartDate;
                Messages.SuccessfullInputSave();
            }
            return booking;
        }

        public Booking ChangeNumberOfGuests(Booking booking)
        {
            Messages.ClearAndShowHeader($"Ändra antal gäster. Nuvarande antal: {booking.NumberOfGuests}");
            Messages.RequiredInputMessage();
            Console.Write("  1. Värdet måste vara mellan 1-255.\n");
            Messages.SetValueWithCursor();

            byte numberOfGuests = 0;
            while (true)
            {
                string? input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                    return booking;

                if (string.IsNullOrWhiteSpace(input))
                    return booking;

                if (byte.TryParse(input, out numberOfGuests))
                {
                    if (numberOfGuests > 0)
                    {
                        booking.NumberOfGuests = numberOfGuests;
                        break;
                    }
                    Console.WriteLine("  Antalet gäster måste vara större än 0.\n  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("  Ogiltig inmatning. Ange ett heltal mellan 1 och 255.\n  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
            }
            return booking;
        }

        public Booking ChangeRoomsInBooking(Booking booking)
        {
            var roomNumbers = booking.ListOfBookingRoomsInBooking
                .Select(br => br.Room.RoomNumber)
                .ToList();
            var roomNumbersString = string.Join(", ", roomNumbers);

            Messages.ClearAndShowHeader($"Nuvarande bokade rum: {roomNumbersString}");
            Console.WriteLine("  Tryck på valfri tangent för att hitta lediga rum baserat på datum och antal gäster...");
            Console.ReadKey();

            var selectedRooms = GetRoomsInBooking(booking.StartDate, booking.EndDate, booking.NumberOfGuests);
            if (selectedRooms == null)
                return booking;

            if (selectedRooms.Any())
            {
                var newBookingRooms = selectedRooms.Select(room => new BookingRoom
                {
                    Booking = booking,
                    BookingId = booking.Id,
                    Room = room,
                    RoomId = room.Id,
                }).ToList();

                booking.ListOfBookingRoomsInBooking.Clear();
                foreach (var newRoom in newBookingRooms)
                {
                    booking.ListOfBookingRoomsInBooking.Add(newRoom);
                }
                return booking;
            }
            else
            {
                Console.WriteLine("\n  Ingen rumskombination vald, avbryter bokning.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                return booking;
            }
        }
        public Booking ChangeOtherInfo(Booking booking)
        {
            Messages.ClearAndShowHeader("Övrig information om bokningen");
            Console.WriteLine("  Ange övrig information om bokningen (valfritt):");
            Console.WriteLine("  Lämna fältet tomt och tryck ENTER om du inte vill ange något.");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor);

            booking.OtherInfoInBooking = Console.ReadLine();

            Messages.SuccessfullInputSave();
            return booking;
        }

        public bool ValidateBooking(Booking booking)
        {
            //if (booking.StartDate < DateTime.Now.Date)
            //{
            //    Console.WriteLine("\n  Startdatum kan inte vara tidigare än dagens datum.\n  Tryck valfri tangent för att återgå och ändra...");
            //    Console.ReadKey();
            //    return false;
            //}
            if (booking.EndDate <= booking.StartDate)
            {
                Console.WriteLine("\n  Slutdatum måste vara senare än startdatum.\n  Tryck valfri tangent för att återgå och ändra...");
                Console.ReadKey();
                return false;
            }

            int totalCapacity = 0;
            foreach (var bookingRoom in booking.ListOfBookingRoomsInBooking)
            {
                var room = bookingRoom.Room;

                bool isRoomAvailable = !_dbContext.Bookings
                    .Include(b => b.ListOfBookingRoomsInBooking)
                    .Any(b => b.Id != booking.Id &&
                              b.ListOfBookingRoomsInBooking.Any(br => br.RoomId == room.Id) &&
                              b.StartDate < booking.EndDate &&
                              b.EndDate > booking.StartDate);

                if (!isRoomAvailable)
                {
                    Console.WriteLine($"  Rummet {room.RoomNumber} är redan bokat för den valda perioden.\n  Tryck valfri tangent för att återgå och ändra...");
                    Console.ReadKey();
                    return false;
                }

                totalCapacity += room.RoomType == BedSize.Double ? 2 : 1;
                totalCapacity += room.NumberOfPossibleExtraBeds;
            }
            if (booking.NumberOfGuests > totalCapacity)
            {
                Console.WriteLine($"  Totala kapaciteten för rummen räcker inte för {booking.NumberOfGuests} gäster.\n  Tryck valfri tangent för att återgå och ändra...");
                Console.ReadKey();
                return false;
            }
            return true;
        }

        public void SaveChangesOnBookingToDataBase(Booking booking)
        {
            Console.WriteLine($"  Bokningen är giltig och kan sparas.\n  Faktura: {booking.InvoiceId} kopplad till bokningen kommer att tas bort och en ny kommer att genereras.\n  Om du är säker, skriv JA, annars tryck enter.");
            string? input = Console.ReadLine();

            if (input?.ToLower() == "ja")
            {
                if (booking.InvoiceInBooking != null)
                {
                    _dbContext.Invoices.Remove(booking.InvoiceInBooking);
                    booking.InvoiceInBooking = null;
                }
                Console.Write("\n  Bokning ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(booking.Id);
                Console.ResetColor();
                Console.WriteLine(" har sparats.");
                Thread.Sleep(500);

                _dbContext.SaveChanges();
                _invoiceService.Value.GenerateInvoiceOfBooking(booking);
                _dbContext.SaveChanges();

                Console.Write("\n  Faktura ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(booking.InvoiceId);
                Console.ResetColor();
                Console.WriteLine(" har skapats.");
                Thread.Sleep(500);

                ReadOneBooking(booking, false, true);
            }
        }
    }
}

