using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.UI;
using HotelApp.Utilities;

namespace HotelApp.Services.BookingService
{
    public class BookingService
    {
        private readonly DisplayList _displayList;
        private readonly ApplicationDbContext_FAKE _dbContext;
        private readonly Lazy<RoomService> _roomService;
        private readonly Lazy<CustomerService> _customerService;
        private readonly Lazy<InvoiceService> _invoiceService;
        public BookingService(DisplayList displayList, ApplicationDbContext_FAKE dbContext, Lazy<RoomService> roomService, Lazy<CustomerService> customerServices, Lazy<InvoiceService> invoiceService)
        {
            _displayList = displayList;
            _dbContext = dbContext;
            _roomService = roomService;
            _customerService = customerServices;
            _invoiceService = invoiceService;
        }
        public void CheckAvailability()
        {
            DateTime startDate;
            DateTime endDate;
            byte numberOfGuests;
            List<Room> selectedRooms = new List<Room>();

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
                return;

            List<List<Room>>? listOfAvailableRoomCombinations = GetAvailableRooms(startDate, endDate, numberOfGuests);
            if (listOfAvailableRoomCombinations == null)
                return;
            List<string> listOfFormattedRoomCombinations = listOfAvailableRoomCombinations
                .Select(combination => _roomService.Value.FormatRoomCombination(combination))
                .ToList();

            int selectedIndexInListOfFormattedRoomCompinations = _displayList.BrowseAList(listOfFormattedRoomCombinations, false, Graphics.GetHeaderAsString("Sökresultat lediga rumskombinationer"), false);

            if (selectedIndexInListOfFormattedRoomCompinations == -1)
                return;
            else if (selectedIndexInListOfFormattedRoomCompinations >= listOfAvailableRoomCombinations.Count)
            {
                Console.WriteLine("  Ogiltigt värde. Avbryter bokning.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                return;
            }
            selectedRooms = listOfAvailableRoomCombinations[selectedIndexInListOfFormattedRoomCompinations];
            var otherInfo = GetOtherInfoAsString();
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

        public string? GetOtherInfoAsString()
        {
            Messages.ClearAndShowHeader("Övrig information om bokningen");
            Console.WriteLine("  Ange övrig information om bokningen (valfritt):");
            Console.WriteLine("  Lämna fältet tomt och tryck ENTER om du inte vill ange något.");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor);
            string ? otherInfo = Console.ReadLine()?.Trim();

            Messages.SuccessfullInput();
            return otherInfo;
        }


        public void StartNewBooking(List<Room> selectedRooms, DateTime startDate, DateTime endDate, byte numberOfGuests, string? otherInfo)
        {
            var customerInNewBooking = _customerService.Value.GetCustomer();
            if (customerInNewBooking == null)
            {
                Console.WriteLine("  Ingen kund kunde hittas.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                Messages.AbortBooking();
                return;
            }
            else if (customerInNewBooking != null)
                CreateNewBooking(selectedRooms, startDate, endDate, numberOfGuests, customerInNewBooking, otherInfo);
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
            
            ReadOneBooking(newBooking, true);
        }

        public void ReadOneBooking(Booking booking, bool isNewFromStart)
        {
            string messageToUseInHeader = $"Visar bokningsnummer {booking.Id}";
            if (isNewFromStart)
                messageToUseInHeader = $"Bokning med bokningsnummer {booking.Id} skapad";

            Console.Clear();
            Graphics.ShowMainGraphics();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();

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

            if (booking.ListOfInvoicesInBooking != null && booking.ListOfInvoicesInBooking.Any())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n           --- Relaterade Fakturor ---");
                foreach (var invoice in booking.ListOfInvoicesInBooking)
                {
                    Console.WriteLine($"           FakturaNr: {invoice.Id}, Belopp: {invoice.TotalAmount:C}, Betald: {(invoice.IsPaid ? "Ja" : "Nej")}, Förfallen: {(invoice.IsOverDue ? "Ja" : "Nej")}");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  Inga relaterade fakturor till denna bokning.");
                Console.ResetColor();
            }

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }


        public void SaveBookingToDataBase(Booking newBooking)
        {
            _dbContext.Bookings.Add(newBooking);
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
                    Room = room,
                    Id = newBooking.Id,
                    RoomNumberAsID = room.RoomNumber
                });
            }

            return newBooking;
        }



        public List<List<Room>>? GetAvailableRooms(DateTime startDate, DateTime endDate, byte numberOfGuests)
        {
            var availableRooms = _dbContext.Rooms
                .Where(r => r.IsActive && IsRoomAvailable(r, startDate, endDate))
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

        private bool IsRoomAvailable(Room room, DateTime startDate, DateTime endDate)
        {
            return room.ListOfBookingRoomsInRoom == null || !room.ListOfBookingRoomsInRoom
                .Any(br => !(endDate <= br.Booking.StartDate || startDate >= br.Booking.EndDate));
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

                    Console.WriteLine("Antalet gäster måste vara större än 0.\n  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Ogiltig inmatning. Ange ett heltal mellan 1 och 255.\n  Tryck på valfri tangent för att fortsätta...");
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

        public void SearchBookingToList(bool isCancel, bool isToUpdateInfo)
        {
            string messageToUseInHeader = "Sök bokning";
            if (isCancel)
                messageToUseInHeader = "Sök för att avboka en bokning";
            if (isToUpdateInfo)
                messageToUseInHeader = "Sök för att lägga till information";

            Messages.ClearAndShowHeader(messageToUseInHeader);
            Messages.RequiredInputMessage();
            Console.WriteLine("   1. Sökbar bokningsinfo: Namn, BokningsNr, Incheckningsdatum/Utcheckningsdatum (YYYY-MM-DD");
            Console.WriteLine("\n  Sök:");
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

            userInput = userInput.ToLower();
            var matchingBookings = _dbContext.Bookings
                .Where(b =>
                    b.CustomerInBooking.FirstName.ToLower().Contains(userInput) ||
                    b.CustomerInBooking.LastName.ToLower().Contains(userInput) ||
                    b.Id.ToString().Contains(userInput) ||
                    (DateTime.TryParse(userInput, out DateTime inputDate) &&
                    (b.StartDate.Date == inputDate || b.EndDate.Date == inputDate)))
                .ToList();

            if (!matchingBookings.Any())
            {
                Console.WriteLine("  Inga bokningar hittades som matchar din sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
            else
                SelectBookingInList(matchingBookings, isCancel, isToUpdateInfo);
        }

        public void SelectBookingInList(List<Booking> matchingBookings, bool isToCancel, bool isToUpdateInfo)
        {
            string messageToUseInHeader = "Sökresultat, välj bokning för att visa all info ↑/↓/↩";
            if (isToCancel)
                messageToUseInHeader = "Sökresultat, välj bokning för att AVBOKA ↑/↓/↩";
            if (isToUpdateInfo)
                messageToUseInHeader = "Sökresultat, välj bokning för att lägga till eller ändra Övrig info ↑/↓/↩";

            var selectedIndex = _displayList.BrowseAList(matchingBookings, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < matchingBookings.Count && isToCancel && !isToUpdateInfo)
                CancelBooking(matchingBookings[selectedIndex]);
            else if (selectedIndex >= 0 && selectedIndex < matchingBookings.Count && !isToCancel && isToUpdateInfo)
                ChangeOtherInfo(matchingBookings[selectedIndex]);
            else if (selectedIndex >= 0 && selectedIndex < matchingBookings.Count && !isToCancel && !isToUpdateInfo)
                ReadOneBooking(matchingBookings[selectedIndex], false);
            else if (selectedIndex == -1)
                return;
            else
            {
                Console.WriteLine("  Fel: Ogiltigt värde i SelectBookingInList.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
        }

        private void ChangeOtherInfo(Booking booking)
        {
            Messages.ClearAndShowHeader("Övrig information om bokningen");
            Console.WriteLine("  Ange övrig information om bokningen (valfritt):");
            Console.WriteLine("  Lämna fältet tomt och tryck ENTER om du inte vill ange något.");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor);
            string? otherInfo = Console.ReadLine()?.Trim();

            // Uppdatera booking med den nya informationen
            booking.OtherInfoInBooking = string.IsNullOrWhiteSpace(otherInfo) ? null : otherInfo;

            // Uppdatera bokningen i dbContext
            //_dbContext.Bookings.Update(booking);
            //_dbContext.SaveChanges();

            Messages.SuccessfullInput();
        }

        //public Booking GetOtherInfo(Booking booking)
        //{
        //    Messages.ClearAndShowHeader("Övrig information om bokningen");
        //    Console.WriteLine("  Ange övrig information om bokningen (valfritt):");
        //    Console.WriteLine("  Lämna fältet tomt och tryck ENTER om du inte vill ange något.");

        //    string? otherInfo = Console.ReadLine()?.Trim();
        //    booking.OtherInfoInBooking = otherInfo;
        //    Messages.SuccessfullInput();
        //    return booking;
        //}

        public void Get100ByStartDate(bool isPastStartDate)
        {
            List<Booking> bookings;

            if (isPastStartDate)
            {
                // Hämta 100 tidigare bokningar sorterade fallande på StartDate
                bookings = _dbContext.Bookings
                    .Where(b => b.StartDate < DateTime.Now)
                    .OrderByDescending(b => b.StartDate)
                    .Take(100)
                    .ToList();
            }
            else
            {
                // Hämta 100 kommande bokningar sorterade stigande på StartDate
                bookings = _dbContext.Bookings
                    .Where(b => b.StartDate >= DateTime.Now)
                    .OrderBy(b => b.StartDate)
                    .Take(100)
                    .ToList();
            }
            Messages.ClearAndShowHeader("Visar Top 100");

            if (!bookings.Any())
            {
                Console.WriteLine("  Inga bokningar hittades för angivet kriterium.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }

            foreach (var booking in bookings)
            {
                Console.WriteLine($"  BokningId: {booking.Id}, StartDate: {booking.StartDate:yyyy-MM-dd}, Kund: {booking.CustomerInBooking.FirstName} {booking.CustomerInBooking.LastName}");
            }

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }

        public void CancelBooking(Booking booking)
        {
            if (booking == null)
            {
                Console.WriteLine("  Bokningen kan inte vara null. Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
                return;
            }

            // Avbryt bokningen MOT DB!!
            booking.IsCancelled = true;

            if (booking.ListOfInvoicesInBooking != null && booking.ListOfInvoicesInBooking.Any())
            {
                foreach (var invoice in booking.ListOfInvoicesInBooking)
                {
                    invoice.IsCancelled = true;
                }
            }
            Console.WriteLine($"  Bokningen med ID {booking.Id} och dess kopplade fakturor har annullerats.");
        }

        public void SearchCurrentVisitors()
        {
            DateTime today = DateTime.Now.Date;

            var currentBookings = _dbContext.Bookings
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
            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine(visitors);
            Console.ResetColor();
            Console.WriteLine("\n  Tryck på valfri tangent för att återgå till menyn...");
            Console.ReadKey();
        }
    }
}

