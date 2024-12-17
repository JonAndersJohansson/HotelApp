
using HotelApp.Controllers;
using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services.BookingService
{
    public class BookingService
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<IMenu> _mainMenu;
        private readonly ApplicationDbContext_FAKE _dbContext;
        private readonly RoomService _roomService;
        private readonly CustomerService _customerService;
        private readonly InvoiceService _invoiceService;
        private readonly Lazy<BookingController> _bookingController;
        public BookingService(DisplayList displayList, Lazy<IMenu> mainMenu, ApplicationDbContext_FAKE dbContext, RoomService roomService, CustomerService customerServices, InvoiceService invoiceService, Lazy<BookingController> bookingController)
        {
            _displayList = displayList;
            _mainMenu = mainMenu;
            _dbContext = dbContext;
            _roomService = roomService;
            _customerService = customerServices;
            _invoiceService = invoiceService;
            _bookingController = bookingController;
        }
        public void CheckAvailability()
        {
            DateTime startDate;
            DateTime endDate;
            byte numberOfGuests;
            List<Room> selectedRooms = new List<Room>();
            //List<List<Room>> listOfAvailableRoomCombinations = new List<Room>();

            while (true)
            {
                startDate = GetStartDate("Välj INCHECKNING");
                if (startDate == DateTime.MinValue)
                    AbortBookingInMainMenu();
                if (startDate < DateTime.Now.Date)
                {
                    Console.WriteLine("  Ogiltig incheckningsdatum. Datumet kan inte vara bakåt i tiden.\n  Tryck på valfri tangent för att försöka igen...");
                    Console.ReadKey();
                    continue;
                }
                Messages.SuccessfullInput();

                endDate = GetStartDate("Välj UTCHECKNING");
                if (startDate == DateTime.MinValue)
                    AbortBookingInMainMenu();
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
                AbortBookingInMainMenu();

            List<List<Room>> listOfAvailableRoomCombinations = GetAvailableRooms(startDate, endDate, numberOfGuests);

            List<string> listOfFormattedRoomCombinations = listOfAvailableRoomCombinations
                .Select(combination => _roomService.FormatRoomCombination(combination))
                .ToList();

            int selectedIndexInListOfFormattedRoomCompinations = _displayList.BrowseAList(listOfFormattedRoomCombinations, false, Graphics.GetHeaderAsString("Sökresultat lediga rumskombinationer"), false);

            if (selectedIndexInListOfFormattedRoomCompinations == -1)
                AbortBookingInMainMenu();
            else if (selectedIndexInListOfFormattedRoomCompinations >= listOfAvailableRoomCombinations.Count)
            {
                Console.WriteLine("  Ogiltigt värde. Avbryter bokning.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                AbortBookingInMainMenu();
            }
            selectedRooms = listOfAvailableRoomCombinations[selectedIndexInListOfFormattedRoomCompinations];
            var otherInfo = GetOtherInfoInBooking();
            if (selectedRooms.Any())
            {
                StartNewBooking(selectedRooms, startDate, endDate, numberOfGuests, otherInfo);
            }
            else
            {
                Console.WriteLine("\n  Ingen rumskombination vald, avbryter bokning.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                AbortBookingInMainMenu();
            }
        }

        public string? GetOtherInfoInBooking()
        {
            Console.Clear();
            Graphics.GetHeaderAsString("Övrig information om bokningen");

            Console.WriteLine("  Ange övrig information om bokningen (valfritt):");
            Console.WriteLine("  Lämna fältet tomt och tryck ENTER om du inte vill ange något.");

            string? otherInfo = Console.ReadLine()?.Trim();

            Messages.SuccessfullInput();
            return otherInfo;
        }


        public void StartNewBooking(List<Room> selectedRooms, DateTime startDate, DateTime endDate, byte numberOfGuests, string? otherInfo)
        {
            var customerInNewBooking = _customerService.GetCustomer();
            CreateNewBooking(selectedRooms, startDate, endDate, numberOfGuests, customerInNewBooking, otherInfo);
        }

        public void CreateNewBooking(List<Room> selectedRooms, DateTime startDate, DateTime endDate, byte numberOfGuests, Customer customer, string? otherInfo)
        {
            Console.WriteLine("CreateNewBooking");
            Console.ReadKey();
            var newBooking = GenerateBookingFromInput(selectedRooms, startDate, endDate, numberOfGuests, customer, otherInfo);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Kund kopplad...");
            Console.ResetColor();
            Thread.Sleep(1000);
            
            SaveBookingToDataBase(newBooking);
            _invoiceService.GenerateInvoiceOfBooking(newBooking);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Faktura skapad...");
            Console.ResetColor();
            Thread.Sleep(1000);
            
            ReadOneBooking(newBooking, true);
        }

        public void ReadOneBooking(Booking booking, bool isNewFromStart)
        {
            string messageToUseInHeader = $"Visar bokningsnummer {booking.BookingId}";
            if (isNewFromStart)
                messageToUseInHeader = $"Bokning med bokningsnummer {booking.BookingId} skapad";

            Console.Clear();
            Graphics.ShowMainGraphics();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();

            // Grundläggande information om bokningen
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  BokningsNr: {booking.BookingId}");
            Console.WriteLine($"  Incheckning: {booking.StartDate:yyyy-MM-dd}");
            Console.WriteLine($"  Utcheckning: {booking.EndDate:yyyy-MM-dd}");
            Console.WriteLine($"  Antal gäster: {booking.NumberOfGuests}");
            Console.WriteLine($"  Annulerad: {(booking.IsCancelled ? "Ja" : "Nej")}");
            Console.WriteLine($"  Övrig information: {booking.OtherInfoInBooking ?? "Ingen"}");
            Console.ResetColor();

            // Koppling till kunden
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n     --- Kundinformation ---");
            Console.WriteLine($"     KundNr: {booking.CustomerInBooking.CustomerId}");
            Console.WriteLine($"     Namn: {booking.CustomerInBooking.FirstName} {booking.CustomerInBooking.LastName}");
            Console.WriteLine($"     Telefonnummer: {booking.CustomerInBooking.PhoneNumber}");
            Console.WriteLine($"     Email: {booking.CustomerInBooking.EmailAddress}");
            Console.ResetColor();

            // Kopplade rum via BookingRoom
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

            // Fakturainformation
            if (booking.ListOfInvoicesInBooking != null && booking.ListOfInvoicesInBooking.Any())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n           --- Relaterade Fakturor ---");
                foreach (var invoice in booking.ListOfInvoicesInBooking)
                {
                    Console.WriteLine($"           FakturaNr: {invoice.InvoiceId}, Belopp: {invoice.TotalAmount:C}, Betald: {(invoice.IsPaid ? "Ja" : "Nej")}, Förfallen: {(invoice.IsOverDue ? "Ja" : "Nej")}");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  Inga relaterade fakturor till denna bokning.");
                Console.ResetColor();
            }

            // Återgå till huvudmenyn
            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _mainMenu.Value.MenuSwitch();
        }


        public void SaveBookingToDataBase(Booking newBooking)
        {
            _dbContext.Bookings.Add(newBooking);
        }

        public Booking GenerateBookingFromInput(List<Room> selectedRooms, DateTime startDate, DateTime endDate, byte numberOfGuests, Customer customer, string? otherInfo)
        {
            if (selectedRooms == null || !selectedRooms.Any())
            {
                throw new ArgumentException("Inga rum valdes. Bokning kan inte genereras.");
            }

            if (numberOfGuests <= 0)
            {
                throw new ArgumentException("Antalet gäster måste vara större än 0.");
            }

            if (startDate >= endDate)
            {
                throw new ArgumentException("Utcheckningsdatum måste vara senare än incheckningsdatum.");
            }

            // Skapa en ny bokning
            var newBooking = new Booking
            {
                StartDate = startDate,
                EndDate = endDate,
                NumberOfGuests = numberOfGuests,
                CustomerInBooking = customer,
                CustomerId = customer.CustomerId,
                OtherInfoInBooking = otherInfo,
                ListOfBookingRoomsInBooking = new List<BookingRoom>()
            };

            // Koppla rum till bokningen via BookingRoom
            foreach (var room in selectedRooms)
            {
                newBooking.ListOfBookingRoomsInBooking.Add(new BookingRoom
                {
                    Booking = newBooking,         // Koppla bokningen
                    Room = room,                  // Koppla rummet
                    BookingId = newBooking.BookingId, // Sätts när bokningen sparas i databasen
                    RoomNumberAsID = room.RoomNumber
                });
            }

            return newBooking;
        }



        public List<List<Room>> GetAvailableRooms(DateTime startDate, DateTime endDate, byte numberOfGuests)
        {
            // Hämta alla aktiva rum
            var availableRooms = _dbContext.Rooms
                .Where(r => r.IsActive && IsRoomAvailable(r, startDate, endDate))
                .ToList();

            // Lista för att lagra möjliga kombinationer av rum
            var roomCombinations = new List<List<Room>>();

            // Generera alla möjliga kombinationer av rum som täcker antalet gäster
            FindRoomCombinations(availableRooms, numberOfGuests, new List<Room>(), roomCombinations);

            if (roomCombinations == null || !roomCombinations.Any())
            {
                Console.WriteLine("  Inga lediga rumskombinationer hittades för det angivna datumet och antalet gäster.\n  Tryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                AbortBookingInMainMenu();
            }
            return roomCombinations;
        }

        private bool IsRoomAvailable(Room room, DateTime startDate, DateTime endDate)
        {
            // Kontrollera att rummet är ledigt för det angivna datumintervallet
            return room.ListOfBookingRoomsInRoom == null || !room.ListOfBookingRoomsInRoom
                .Any(br => !(endDate <= br.Booking.StartDate || startDate >= br.Booking.EndDate));
        }

        private void FindRoomCombinations(List<Room> rooms, int remainingGuests, List<Room> currentCombination, List<List<Room>> result)
        {
            if (remainingGuests <= 0)
            {
                // Om vi täckt alla gäster, lägg till kombinationen i resultatet
                result.Add(new List<Room>(currentCombination));
                return;
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                int roomCapacity = GetRoomCapacity(room);

                if (roomCapacity >= remainingGuests || roomCapacity > 0)
                {
                    // Lägg till rummet i nuvarande kombination
                    currentCombination.Add(room);

                    // Rekursiv sökning med uppdaterat gästantal och resterande rum
                    FindRoomCombinations(rooms.Skip(i + 1).ToList(), remainingGuests - roomCapacity, currentCombination, result);

                    // Ta bort det senast tillagda rummet (backtracking)
                    currentCombination.RemoveAt(currentCombination.Count - 1);
                }
            }
        }
        private int GetRoomCapacity(Room room)
        {
            // Beräkna rummets kapacitet baserat på typ och extra sängar
            int baseCapacity = room.RoomType == BedSize.Double ? 2 : 1;
            return baseCapacity + room.NumberOfPossibleExtraBeds;
        }


        public byte GetNumberOfGuests()
        {
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString("Antal gäster"));
            Console.ResetColor();
            Messages.RequiredInputMessage();
            Console.Write("  1. Värdet måste vara mellan 1-255.\n");
            Messages.SetValueWithCursor();

            byte numberOfGuests = 0;
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                    AbortBookingInMainMenu();

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

        public void AbortBookingInMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n  Avbryter bokning...");
            Console.ResetColor();
            Thread.Sleep(1000);
            _mainMenu.Value.MenuSwitch();
        }

        public DateTime GetEndDate(string headerMessage)
        {
            var selectedEndDate = Calendar.GetDateTimeByCalendar(headerMessage);
            return selectedEndDate;
        }

        public DateTime GetStartDate(string headerMessage)
        {
            var selectedStartDate = Calendar.GetDateTimeByCalendar(headerMessage);
            return selectedStartDate;
        }

        public List<Booking> GetListOfBookingsBySearch(bool isCancel, bool isToUpdateInfo)
        {
            string messageToUseInHeader = "Sök kund";
            if (isCancel)
                messageToUseInHeader = "Sök för att avboka en bokning";
            if (isToUpdateInfo)
                messageToUseInHeader = "Sök för att lägga till information";
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();
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
                _bookingController.Value.MenuSwitch();
                return new List<Booking>();
            }
            if (userInput.ToLower() == "exit")
            {
                _bookingController.Value.MenuSwitch();
                return new List<Booking>();
            }

            userInput = userInput.ToLower();
            var matchingBookings = _dbContext.Bookings
                .Where(b =>
                    b.CustomerInBooking.FirstName.ToLower().Contains(userInput) ||
                    b.CustomerInBooking.LastName.ToLower().Contains(userInput) ||
                    b.BookingId.ToString().Contains(userInput) ||
                    (DateTime.TryParse(userInput, out DateTime inputDate) &&
                    (b.StartDate.Date == inputDate || b.EndDate.Date == inputDate)))
                .ToList();

            if (!matchingBookings.Any())
            {
                Console.WriteLine("  Inga bokningar hittades som matchar din sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
            }

            return matchingBookings;
        }

        public Booking GetABookingInList(List<Booking> matchingBookings, bool isToCancel, bool isToUpdateInfo)
        {
            string messageToUseInHeader = "Sökresultat, välj bokning för att visa all info ↑/↓/↩";
            if (isToCancel)
                messageToUseInHeader = "Sökresultat, välj bokning för att AVBOKA ↑/↓/↩";
            if (isToUpdateInfo)
                messageToUseInHeader = "Sökresultat, välj bokning för att lägga till eller ändra Övrig info ↑/↓/↩";

            var selectedIndex = _displayList.BrowseAList(matchingBookings, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < matchingBookings.Count)
                return matchingBookings[selectedIndex];
            else if (selectedIndex == -1)
            {
                _bookingController.Value.MenuSwitch();
                return matchingBookings[-1];
            }
            else
                return matchingBookings[-1];
        }
    }
}

