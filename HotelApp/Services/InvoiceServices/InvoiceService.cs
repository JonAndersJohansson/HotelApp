using HotelApp.UI;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelApp.Data;
using HotelApp.Controllers;
using HotelApp.Data.Models;
using System.Globalization;
using Calendar = HotelApp.Utilities.Calendar;
using HotelApp.UI.Menus;

namespace HotelApp.Services.InvoiceServices
{
    public class InvoiceService
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<InvoiceController> _invoiceController;
        private ApplicationDbContext_FAKE _dbContext;
        public InvoiceService(DisplayList displayList, Lazy<InvoiceController> invoiceController, ApplicationDbContext_FAKE dbContext)
        {
            _displayList = displayList;
            _invoiceController = invoiceController;
            _dbContext = dbContext;
        }
        public Invoice GetAInvoiceFrom100IsPaid(bool isPaid)
        {
            var top100Invoices = _dbContext.Invoices
                .Where(i => i.IsPaid == isPaid)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(100)
                .ToList();
            var selectedIndex = _displayList.BrowseAList(top100Invoices, false, Graphics.GetHeaderAsString("Visar 100 senaste BETALDA fakturorna. Välj en för att visa all info ↑/↓/↩"), false);
            if (selectedIndex >= 0 && selectedIndex < top100Invoices.Count)
                return top100Invoices[selectedIndex];
            else
            {
                _invoiceController.Value.MenuSwitch();
                return top100Invoices[-1];
            }
        }
        public Invoice GetAInvoiceFrom100IsOverDue()
        {
            var top100Invoices = _dbContext.Invoices
                .Where(i => i.IsOverDue == true)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(100)
                .ToList();
            var selectedIndex = _displayList.BrowseAList(top100Invoices, false, Graphics.GetHeaderAsString("Visar 100 senaste förfallna fakturorna. Välj en för att visa all info ↑/↓/↩"), false);
            if (selectedIndex >= 0 && selectedIndex < top100Invoices.Count)
                return top100Invoices[selectedIndex];
            else
            {
                _invoiceController.Value.MenuSwitch();
                return top100Invoices[-1];
            }
        }

        public List<Invoice> GetListOfInvoiceBySearch(bool isToCancel)
        {
            string messageToUseInHeader = "Sök faktura";
            if (isToCancel)
                messageToUseInHeader = "Sök för att annulera faktura";

            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();
            Messages.RequiredInputMessage();
            Console.WriteLine("   1. Sökbar fakturainfo: FakturaNr, Fakturadatum (yyyy-mm-dd)");
            Console.WriteLine("\n  Sök:");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(7, currentLineCursor - 1);
            string? userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("  Inga fakturor hittades som matchar din sökning.");
                _invoiceController.Value.MenuSwitch();
                return new List<Invoice>();
            }
            if (userInput.ToLower() == "exit")
            {
                _invoiceController.Value.MenuSwitch();
                return new List<Invoice>();
            }
            List<Invoice> matchingInvoices;
            if (userInput.Length < 10)
            {
                matchingInvoices = _dbContext.Invoices
                .Where(i =>
                    i.InvoiceId.ToString().Contains(userInput))
                .ToList();
            }
            else
            {
                matchingInvoices = _dbContext.Invoices
                .Where(i =>
                    i.InvoiceDate.ToString("yyyy-MM-dd").Contains(userInput))
                .ToList();
            }
            if (!matchingInvoices.Any())
            {
                Console.WriteLine("  Inga fakturor hittades som matchar din sökning.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
            }

            return matchingInvoices;
        }
        public Invoice GetInvoiceInList(List<Invoice> matchingInvoices, bool isToCancel)
        {

            string messageToUseInHeader = "Sökresultat, välj faktura för att visa all info ↑/↓/↩";
            if (isToCancel)
                messageToUseInHeader = "Sökresultat, välj faktura för att annulera ↑/↓/↩";

            var selectedIndex = _displayList.BrowseAList(matchingInvoices, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < matchingInvoices.Count)
                return matchingInvoices[selectedIndex];
            else if (selectedIndex == -1)
            {
                _invoiceController.Value.MenuSwitch();
                return matchingInvoices[-1];
            }
            else
                return matchingInvoices[-1];
        }

        public void ReadOneInvoice(Invoice selectedInvoice)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString($"  Info fakturanummer: {selectedInvoice.InvoiceId}"));
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  FakturaNr: {selectedInvoice.InvoiceId}");
            Console.WriteLine($"  Fakturadatum: {selectedInvoice.InvoiceDate}");
            Console.WriteLine($"  Belopp: {selectedInvoice.TotalAmount}");
            Console.WriteLine($"  Förfallen: {(selectedInvoice.IsOverDue ? "Ja" : "Nej")}");
            Console.WriteLine($"  Betald: {(selectedInvoice.IsPaid ? "Ja" : "Nej")}");
            Console.WriteLine($"  Annulerad: {(selectedInvoice.IsCancelled ? "Ja" : "Nej")}");
            Console.ResetColor();

            if (selectedInvoice.BookingInInvoice != null)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n  --- Bokning kopplad till FakNr:{selectedInvoice.InvoiceId} ---");

                Console.WriteLine($"      Bokningnummer: {selectedInvoice.BookingInInvoice.BookingId}");
                Console.WriteLine($"      Antal besökare: {selectedInvoice.BookingInInvoice.NumberOfGuests}");
                Console.WriteLine($"      Incheckning: {selectedInvoice.BookingInInvoice.StartDate}");
                Console.WriteLine($"      Utcheckning: {selectedInvoice.BookingInInvoice.EndDate}");
                Console.WriteLine($"      KundNamn: {selectedInvoice.BookingInInvoice.CustomerInBooking.FirstName} {selectedInvoice.BookingInInvoice.CustomerInBooking.LastName}");
                Console.WriteLine($"      KundNr: {selectedInvoice.BookingInInvoice.CustomerInBooking.CustomerId}");

            }
            else
            {
                Console.WriteLine("\n  Inga relaterade bokningar.");
            }
            Console.ResetColor();


            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _invoiceController.Value.MenuSwitch();
        }
        public void RegistratePaymentOnInvoice(Invoice selectedInvoice)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString("Registrera betalning på faktura"));
            Console.ResetColor();

            if (selectedInvoice.IsPaid == false)
            {
                selectedInvoice.IsPaid = true;
                Console.Write("\n  Följande faktura är registrerad som betald: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(selectedInvoice.InvoiceId);
                Console.ResetColor();
            }
            else
                Console.WriteLine("  Fakturan är redan registrerad som betald.");

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _invoiceController.Value.MenuSwitch();
        }
        public void CancelInvoice(Invoice selectedInvoice)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString("Annullera Faktura"));
            Console.ResetColor();

            if (selectedInvoice.IsCancelled == false)
            {
                selectedInvoice.IsCancelled = true;
                Console.Write("\n  Följande faktura är annulerad: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(selectedInvoice.InvoiceId);
                Console.ResetColor();
            }
            else
                Console.WriteLine("  Fakturan är redan annullerad.");

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _invoiceController.Value.MenuSwitch();
        }

        //public Invoice GetInvoiceDate(Invoice invoice, bool isNew)
        //{
        //    string messageToUseInHeader = $"Uppdatera ett fakturadatum";
        //    if (isNew)
        //        messageToUseInHeader = $"Ange fakturadatum";

        //    while (true)
        //    {
        //        Console.Clear();
        //        Graphics.ShowMainGraphics();
        //        Console.ForegroundColor = ConsoleColor.Blue;
        //        Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
        //        Console.ResetColor();


        //        DateTime invoiceDateInput = Calendar.GetDateTimeByCalendar();

        //        if (invoiceDateInput == DateTime.MinValue)
        //        {
        //            return invoice;
        //        }
        //        Messages.RequiredInputMessage();
        //        Console.WriteLine("  1. Datumet måste vara i formatet: YYYY-MM-DD");

        //        if (!isNew)
        //        {
        //            Console.WriteLine($"  Nuvarande fakturadatum:\n  {invoice.InvoiceDate}\n\n  Uppdatera en fakturas fakturadatum:");
        //            int currentLineCursor = Console.CursorTop;
        //            Console.SetCursorPosition(35, currentLineCursor - 1);
        //        }
        //        else
        //            Messages.SetValueWithCursor();

        //        string? invoiceDateInput = Console.ReadLine();
        //        if (invoiceDateInput.ToLower() == "exit")
        //            return invoice;
        //        if (DateTime.TryParseExact(invoiceDateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime invoiceDate))
        //        {
        //            if (invoiceDate > DateTime.Now)
        //                Console.WriteLine("  Fakturadatumet kan inte vara i framtiden.");
        //            else
        //            {
        //                invoice.InvoiceDate = invoiceDate;
        //                Messages.SuccessfullInput();
        //                return invoice;
        //            }
        //        }
        //        else
        //            Console.WriteLine("  Ogiltigt format. Ange datum i formatet YYYY-MM-DD.");

        //        Console.WriteLine("\n  Tryck valfri tangent för att försöka igen...");
        //        Console.ReadKey();
        //    }
        //}

        public Invoice GetTotalAmount(Invoice invoice)
        {
            decimal totalAmount;
            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString("Fakturabelopp ny faktura"));
                Console.ResetColor();
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Beloppet måste vara en siffra mellan 0 och 10000000");

                Messages.SetValueWithCursor();

                string totalAmountInput = Console.ReadLine();
                if (totalAmountInput.ToLower() == "exit")
                    return invoice;

                if (decimal.TryParse(totalAmountInput, out totalAmount))
                {
                    if (totalAmount >= 0 && totalAmount <= 10000000)
                    {
                        invoice.TotalAmount = totalAmount;
                        Messages.SuccessfullInput();
                        return invoice;
                    }
                    else
                        Console.WriteLine("\n  Värdet måste vara mellan 0 och 10000000. Försök igen.");
                }
                else
                    Console.WriteLine("\n  Ogiltig inmatning. Ange ett giltigt värde.");
            }
        }

        public Invoice GetIsPaid(Invoice invoice)
        {
            List<string> listOfChoices = new List<string>
            {
            "Betald", "Ej betald"
            };
            var selectedIndex = _displayList.BrowseAList(listOfChoices, false, Graphics.GetHeaderAsString($"Ange Betald / Obetald. Standardvärde: {(invoice.IsPaid ? "JA" : "NEJ")}"), false);
            if (selectedIndex == -1)
                return invoice;
            else if (selectedIndex == 0)
            {
                invoice.IsPaid = true;
                Messages.SuccessfullInput();
                return invoice;
            }
            else if (selectedIndex == 1)
            {
                invoice.IsPaid = false;
                Messages.SuccessfullInput();
                return invoice;
            }
            else
                return invoice;
        }

        public Invoice GetIsOverDue(Invoice invoice)
        {
            List<string> listOfChoices = new List<string>
            {
            "Förfallen", "Pågående"
            };
            var selectedIndex = _displayList.BrowseAList(listOfChoices, false, Graphics.GetHeaderAsString($"Ange Pågående / Förfallen. Standardvärde: {(invoice.IsOverDue ? "JA" : "NEJ")}"), false);
            if (selectedIndex == -1)
                return invoice;
            else if (selectedIndex == 0)
            {
                invoice.IsOverDue = true;
                Messages.SuccessfullInput();
                return invoice;
            }
            else if (selectedIndex == 1)
            {
                invoice.IsOverDue = false;
                Messages.SuccessfullInput();
                return invoice;
            }
            else
                return invoice;
        }

        public bool ValidateInvoice(Invoice invoice)
        {
            if (invoice.TotalAmount == 0 || invoice.BookingId == 0)
                return false;
            //if (_dbContext.Invoices.Any(i => i.InvoiceId == invoice.InvoiceId) && isNew == true)
            //{
            //    Console.WriteLine("\n  Ett faktura med detta fakturanummer finns redan.\n  Tryck på valfri tangent för att försöka igen...");
            //    Console.ReadKey();
            //    return false;
            //}
            return true;
        }

        public void AddInvoice(Invoice invoice)
        {
            invoice.InvoiceDate = DateTime.Now;

            if (invoice.DueDate == DateTime.MinValue)
                invoice.DueDate = DateTime.Now.AddDays(+30);

            _dbContext.Invoices.Add(invoice);

            Console.Write("\n  Faktura ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(invoice.InvoiceId);
            Console.ResetColor();
            Console.WriteLine(" har sparats.");
            Thread.Sleep(1000);
            ReadOneInvoice(invoice);
        }

        public Invoice GetBookingNr(Invoice invoice)
        {
            int bookingId;
            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString("Koppla faktura mot boknigsnummer"));
                Console.ResetColor();
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Bokningnummer är possitiva. \n\nSkriv in bokningsnummret du vill koppla till fakturan.");

                Messages.SetValueWithCursor();

                string bookingNumberInput = Console.ReadLine();
                if (bookingNumberInput.ToLower() == "exit")
                    return invoice;

                if (int.TryParse(bookingNumberInput, out bookingId))
                {
                    if (bookingId >= 0)
                    {
                        var booking = _dbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

                        if (booking != null)
                        {
                            invoice.BookingId = bookingId;
                            Messages.SuccessfullInput();
                            return invoice;
                        }
                        else
                            Console.WriteLine("\n  Det finns ingen bokning som matchar det numret.");
                    }
                    else
                        Console.WriteLine("\n  Värdet måste vara positivt.");
                }
                else
                    Console.WriteLine("\n  Ogiltig inmatning. Ange ett giltigt värde.");
            }

        }

        public Invoice GetDueDate(Invoice invoice)
        {
            List<string> listOfDueDate = new List<string>
            {
            "10-dagar", "20-dagar", "30-dagar"
            };
            var selectedIndex = _displayList.BrowseAList(listOfDueDate, false, Graphics.GetHeaderAsString($"Välj betalningsvillkor. Standardvärde: 30-dagar"), false);
            if (selectedIndex == -1)
                return invoice;
            else if (selectedIndex == 0)
            {
                invoice.DueDate = DateTime.Now.AddDays(+10);
                Messages.SuccessfullInput();
                return invoice;
            }
            else if (selectedIndex == 1)
            {
                invoice.DueDate = DateTime.Now.AddDays(+20);
                Messages.SuccessfullInput();
                return invoice;
            }
            else if (selectedIndex == 2)
            {
                invoice.DueDate = DateTime.Now.AddDays(+30);
                Messages.SuccessfullInput();
                return invoice;
            }
            else
                return invoice;
        }

        public void GenerateInvoiceOfBooking(Booking newBooking)
        {
            if (newBooking == null)
                throw new ArgumentNullException(nameof(newBooking), "  Bokningen kan inte vara null.");

            if (newBooking.ListOfBookingRoomsInBooking == null || !newBooking.ListOfBookingRoomsInBooking.Any())
                throw new ArgumentException("  Bokningen har inga kopplade rum.");

            // 1. Sätt InvoiceDate till dagens datum
            DateTime invoiceDate = DateTime.Now.Date;

            // 2. Beräkna DueDate baserat på kundens medlemsnivå
            int dueDays = newBooking.CustomerInBooking.Membership switch
            {
                TypeOfMembership.Brons => 10,
                TypeOfMembership.Silver => 20,
                TypeOfMembership.Guld => 30,
                _ => 10 // Standardvärde om något är felaktigt
            };
            DateTime dueDate = invoiceDate.AddDays(dueDays);

            // 3. Beräkna TotalAmount baserat på rum och rabattnivå
            decimal totalAmount = CalculateTotalAmount(newBooking);

            // Skapa fakturan
            var invoice = new Invoice
            {
                InvoiceDate = invoiceDate,
                DueDate = dueDate,
                TotalAmount = totalAmount,
                IsPaid = false,
                IsCancelled = false,
                BookingId = newBooking.BookingId,
                BookingInInvoice = newBooking
            };

            // Spara fakturan i databasen
            SaveInvoiceToDataBase(invoice);
        }
        public decimal CalculateTotalAmount(Booking booking)
        {
            int numberOfNights = (booking.EndDate - booking.StartDate).Days;

            // Beräkna rumskostnader
            decimal roomCost = booking.ListOfBookingRoomsInBooking
                .Sum(bookingRoom => bookingRoom.Room.CostPerNight * numberOfNights);

            // Beräkna rabatt baserat på medlemskap
            decimal discountPercentage = booking.CustomerInBooking.Membership switch
            {
                TypeOfMembership.Brons => 0m,   // Ingen rabatt
                TypeOfMembership.Silver => 0.05m, // 5% rabatt
                TypeOfMembership.Guld => 0.10m,  // 10% rabatt
                _ => 0m
            };

            decimal totalWithDiscount = roomCost * (1 - discountPercentage);
            return Math.Round(totalWithDiscount, 2); // Avrunda till 2 decimaler
        }


        public void SaveInvoiceToDataBase(Invoice newInvoice)
        {
            _dbContext.Invoices.Add(newInvoice);
            
        }
    }
}
