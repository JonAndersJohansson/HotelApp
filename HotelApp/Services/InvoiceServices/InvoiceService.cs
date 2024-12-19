using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.UI;
using HotelApp.Utilities;

namespace HotelApp.Services.InvoiceServices
{
    public class InvoiceService
    {
        private readonly DisplayList _displayList;
        private ApplicationDbContext_FAKE _dbContext;
        public InvoiceService(DisplayList displayList, ApplicationDbContext_FAKE dbContext)
        {
            _displayList = displayList;
            _dbContext = dbContext;
        }
        public void GetAInvoiceFrom100IsPaid(bool isPaid)
        {
            var top100Invoices = _dbContext.Invoices
                .Where(i => i.IsPaid == isPaid)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(100)
                .ToList();
            var selectedIndex = _displayList.BrowseAList(top100Invoices, false, Graphics.GetHeaderAsString("Visar 100 senaste BETALDA fakturorna. Välj en för att visa all info ↑/↓/↩"), false);
            if (selectedIndex >= 0 && selectedIndex < top100Invoices.Count)
                ReadOneInvoice(top100Invoices[selectedIndex]);
            else if (selectedIndex < -1 || selectedIndex > top100Invoices.Count)
            {
                Console.WriteLine("  Fel: Index kunde inte hittas i GetAInvoiceFrom100IsPaid.\n  Tryck valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
        }
        public void GetAInvoiceFrom100IsOverDue()
        {
            var top100Invoices = _dbContext.Invoices
                .Where(i => i.IsOverDue == true)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(100)
                .ToList();
            var selectedIndex = _displayList.BrowseAList(top100Invoices, false, Graphics.GetHeaderAsString("Visar 100 senaste förfallna fakturorna. Välj en för att visa all info ↑/↓/↩"), false);
            if (selectedIndex >= 0 && selectedIndex < top100Invoices.Count)
                ReadOneInvoice(top100Invoices[selectedIndex]);
            else if (selectedIndex == -1)
                return;
            else
            {
                Console.WriteLine("  Fel: Ogiltigt val i GetAInvoiceFrom100IsOverDue.\n  Tryck valfri tangent för att fortsätta");
                Console.ReadKey();
            }
        }

        public void GetListOfInvoiceBySearch(bool isToCancel, bool isToRegistratePay)
        {
            string messageToUseInHeader = "Sök Faktura";
            if (isToCancel) messageToUseInHeader = "Sök faktura att ANNULERA";
            if (isToRegistratePay) messageToUseInHeader = "Sök faktura för att registrera den som betald";

            Messages.ClearAndShowHeader(messageToUseInHeader);
            Messages.RequiredInputMessage();
            Console.WriteLine("   1. Sökbar fakturainfo: FakturaNr, Fakturadatum (yyyy-MM-dd)");
            Console.WriteLine("\n  Sök:");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(7, currentLineCursor - 1);

            string? userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("  Du har inte angivit något.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
            else if (userInput.ToLower() == "exit")
                return;

            List<Invoice> matchingInvoices;
            if (userInput.Length < 10)
            {
                matchingInvoices = _dbContext.Invoices
                    .Where(i => i.Id.ToString().Contains(userInput))
                    .ToList();
            }
            else
            {
                matchingInvoices = _dbContext.Invoices
                    .Where(i => i.InvoiceDate.ToString("yyyy-MM-dd").Contains(userInput))
                    .ToList();
            }
            if (!matchingInvoices.Any())
            {
                Console.WriteLine("  Inga fakturor hittades som matchar din sökning.\n  Tryck på valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
            GetInvoiceInList(matchingInvoices, isToCancel, isToRegistratePay);
        }

        public void GetInvoiceInList(List<Invoice> matchingInvoices, bool isToCancel, bool isToRegistratePay)
        {
            string messageToUseInHeader = "Sökresultat, välj faktura för att visa info ↑/↓/↩";
            if (isToCancel) messageToUseInHeader = "Sökresultat, välj faktura för att annulera ↑/↓/↩";
            if (isToRegistratePay) messageToUseInHeader = "Sökresultat, välj faktura för att registrera den som betald ↑/↓/↩";

            var selectedIndex = _displayList.BrowseAList(matchingInvoices, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);

            if (selectedIndex >= 0 && selectedIndex < matchingInvoices.Count && !isToCancel && !isToRegistratePay)
                ReadOneInvoice(matchingInvoices[selectedIndex]);
            else if (selectedIndex >= 0 && selectedIndex < matchingInvoices.Count && isToCancel && !isToRegistratePay)
                CancelInvoice(matchingInvoices[selectedIndex]);
            else if (selectedIndex >= 0 && selectedIndex < matchingInvoices.Count && !isToCancel && isToRegistratePay)
                RegistratePaymentOnInvoice(matchingInvoices[selectedIndex]);
            else if (selectedIndex == -1)
                return;
            else
            {
                Console.WriteLine("  Fel: inget index kunde hittas i GetInvoiceInList.\n  Tryckvalfri tangent för att återgå...");
                Console.ReadKey();
            }
        }

        public void ReadOneInvoice(Invoice selectedInvoice)
        {
            Messages.ClearAndShowHeader($"  Info fakturanummer: {selectedInvoice.Id}");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  FakturaNr: {selectedInvoice.Id}");
            Console.WriteLine($"  Fakturadatum: {selectedInvoice.InvoiceDate}");
            Console.WriteLine($"  Belopp: {selectedInvoice.TotalAmount}");
            Console.WriteLine($"  Förfallen: {(selectedInvoice.IsOverDue ? "Ja" : "Nej")}");
            Console.WriteLine($"  Betald: {(selectedInvoice.IsPaid ? "Ja" : "Nej")}");
            Console.WriteLine($"  Annulerad: {(selectedInvoice.IsCancelled ? "Ja" : "Nej")}");
            Console.ResetColor();

            if (selectedInvoice.BookingInInvoice != null)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n  --- Bokning kopplad till FakNr:{selectedInvoice.Id} ---");

                Console.WriteLine($"      Bokningnummer: {selectedInvoice.BookingInInvoice.Id}");
                Console.WriteLine($"      Antal besökare: {selectedInvoice.BookingInInvoice.NumberOfGuests}");
                Console.WriteLine($"      Incheckning: {selectedInvoice.BookingInInvoice.StartDate}");
                Console.WriteLine($"      Utcheckning: {selectedInvoice.BookingInInvoice.EndDate}");
                Console.WriteLine($"      KundNamn: {selectedInvoice.BookingInInvoice.CustomerInBooking.FirstName} {selectedInvoice.BookingInInvoice.CustomerInBooking.LastName}");
                Console.WriteLine($"      KundNr: {selectedInvoice.BookingInInvoice.CustomerInBooking.Id}");
            }
            else
                Console.WriteLine("\n  Inga relaterade bokningar.");
            Console.ResetColor();

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }
        public void RegistratePaymentOnInvoice(Invoice selectedInvoice)
        {
            Messages.ClearAndShowHeader("Registrera betalning på faktura");

            if (selectedInvoice.IsPaid == false)
            {
                selectedInvoice.IsPaid = true;
                Console.Write("\n  Följande faktura är registrerad som betald: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(selectedInvoice.Id);
                Console.ResetColor();
            }
            else
                Console.WriteLine("  Fakturan är redan registrerad som betald.");

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }
        public void CancelInvoice(Invoice selectedInvoice)
        {
            Messages.ClearAndShowHeader("Annullera Faktura");

            if (selectedInvoice.IsCancelled == false)
            {
                selectedInvoice.IsCancelled = true;
                Console.Write("\n  Följande faktura är annulerad: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(selectedInvoice.Id);
                Console.ResetColor();
            }
            else
                Console.WriteLine("  Fakturan är redan annullerad.");

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }
        public Invoice GetTotalAmount(Invoice invoice)
        {
            decimal totalAmount;
            while (true)
            {
                Messages.ClearAndShowHeader("Fakturabelopp ny faktura");
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Beloppet måste vara en siffra mellan 0 och 10000000");
                Messages.SetValueWithCursor();

                string? totalAmountInput = Console.ReadLine();
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
            Console.Write(invoice.Id);
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
                Messages.ClearAndShowHeader("Koppla faktura mot boknigsnummer");
                Messages.RequiredInputMessage();
                Console.WriteLine("   1. Bokningnummer är possitiva. \n\nSkriv in bokningsnummret du vill koppla till fakturan.");
                Messages.SetValueWithCursor();

                string? bookingNumberInput = Console.ReadLine();
                if (bookingNumberInput.ToLower() == "exit")
                    return invoice;

                if (int.TryParse(bookingNumberInput, out bookingId))
                {
                    if (bookingId >= 0)
                    {
                        var booking = _dbContext.Bookings.FirstOrDefault(b => b.Id == bookingId);            //DB

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

            DateTime invoiceDate = DateTime.Now.Date;

            int dueDays = newBooking.CustomerInBooking.Membership switch
            {
                TypeOfMembership.Brons => 10,
                TypeOfMembership.Silver => 20,
                TypeOfMembership.Guld => 30,
                _ => 10 
            };
            DateTime dueDate = invoiceDate.AddDays(dueDays);

            decimal totalAmount = CalculateTotalAmount(newBooking);

            var invoice = new Invoice
            {
                InvoiceDate = invoiceDate,
                DueDate = dueDate,
                TotalAmount = totalAmount,
                IsPaid = false,
                IsCancelled = false,
                BookingId = newBooking.Id,
                BookingInInvoice = newBooking
            };
            SaveInvoiceToDataBase(invoice);
        }
        public decimal CalculateTotalAmount(Booking booking)
        {
            int numberOfNights = (booking.EndDate - booking.StartDate).Days;

            decimal roomCost = booking.ListOfBookingRoomsInBooking
                .Sum(bookingRoom => bookingRoom.Room.CostPerNight * numberOfNights);

            decimal discountPercentage = booking.CustomerInBooking.Membership switch
            {
                TypeOfMembership.Brons => 0m,
                TypeOfMembership.Silver => 0.05m,
                TypeOfMembership.Guld => 0.10m,
                _ => 0m
            };

            decimal totalWithDiscount = roomCost * (1 - discountPercentage);
            return Math.Round(totalWithDiscount, 2);
        }


        public void SaveInvoiceToDataBase(Invoice newInvoice)
        {
            _dbContext.Invoices.Add(newInvoice);
        }
    }
}
