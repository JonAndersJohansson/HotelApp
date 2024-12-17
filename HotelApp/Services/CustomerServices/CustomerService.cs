using HotelApp.Controllers;
using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services.CustomerServices
{
    public class CustomerService
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<CustomerController> _customerController;
        private ApplicationDbContext_FAKE _dbContext;
        private readonly Lazy<IMenu> _mainMenu;
        private readonly Lazy<CustomerPropertySelector> _customerPropertySelector;
        public CustomerService(DisplayList displayList, Lazy<CustomerController> customerController, ApplicationDbContext_FAKE dbContext, Lazy<IMenu> mainMenu, Lazy<CustomerPropertySelector> customerPropertySelector)
        {
            _displayList = displayList;
            _customerController = customerController;
            _dbContext = dbContext;
            _mainMenu = mainMenu;
            _customerPropertySelector = customerPropertySelector;
        }
        public List<Customer> GetListOfCustomersBySearch(bool isDeactivate, bool isToChange)
        {
            string messageToUseInHeader = "Sök kund";
            if (isDeactivate)
                messageToUseInHeader = "Sök för att Aktivera/Avaktivera kund";
            if (isToChange)
                messageToUseInHeader = "Sök för att ändra kunduppgifter";
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();
            Messages.RequiredInputMessage();
            Console.WriteLine("   1. Sökbar kundinfo: Namn, KundNr, Telnr, Epost");
            Console.WriteLine("\n  Sök:");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(7, currentLineCursor - 1);
            string? userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("  Inga kunder hittades som matchar din sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
                _customerController.Value.MenuSwitch();
                return new List<Customer>();
            }
            if (userInput.ToLower() == "exit")
            {
                _customerController.Value.MenuSwitch();
                return new List<Customer>();
            }

            userInput = userInput.ToLower();
            var matchingCustomers = _dbContext.Customers
                .Where(c =>
                    c.FirstName.ToLower().Contains(userInput) ||
                    c.LastName.ToLower().Contains(userInput) ||
                    c.PhoneNumber.ToLower().Contains(userInput) ||
                    c.EmailAddress.ToLower().Contains(userInput) ||
                    c.CustomerId.ToString().Contains(userInput))
                .ToList();

            if (!matchingCustomers.Any())
            {
                Console.WriteLine("  Inga kunder hittades som matchar din sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
            }

            return matchingCustomers;
        }
        public Customer GetACustomerInList(List<Customer> matchingCustomers, bool isDeactivate, bool isToChange)
        {

            string messageToUseInHeader = "Sökresultat, välj kund för att visa all info ↑/↓/↩";
            if (isDeactivate)
                messageToUseInHeader = "Sökresultat, välj kund för att Avaktivera / Aktivera ↑/↓/↩";
            if (isToChange)
                messageToUseInHeader = "Sökresultat, välj kund att ändra ↑/↓/↩";

            var selectedIndex = _displayList.BrowseAList(matchingCustomers, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < matchingCustomers.Count)
                return matchingCustomers[selectedIndex];
            else if (selectedIndex == -1)
            {
                _customerController.Value.MenuSwitch();
                return matchingCustomers[-1];
            }
            else
                return matchingCustomers[-1];
        }

        //public Customer GetACustomer(int customerIndex)
        //{
        //    var selectedCustomer = _dbContext.Customers[customerIndex];
        //    return selectedCustomer;
        //}

        public void ReadOneCustomer(Customer selectedCustomer)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString($"  Info kundnummer: {selectedCustomer.CustomerId}"));
            Console.ResetColor();

            // Visa all information om rummet
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  Kundnummer: {selectedCustomer.CustomerId}");
            Console.WriteLine($"  Namn: {selectedCustomer.LastName}, {selectedCustomer.FirstName}");
            Console.WriteLine($"  Adress: {selectedCustomer.Address}");
            Console.WriteLine($"  Telefon nr: {selectedCustomer.PhoneNumber}");
            Console.WriteLine($"  Email: {selectedCustomer.EmailAddress}");
            Console.WriteLine($"  Kundnivå: {selectedCustomer.Membership}");
            Console.WriteLine($"  Födelsedatum: {selectedCustomer.DateOfBirth}");
            Console.WriteLine($"  Övrig info: {selectedCustomer.OtherInfoInCustomer}");
            Console.ResetColor();
            // Visa relaterade bokningar (om några finns)
            if (selectedCustomer.ListOfBookingsInCustomer?.Count > 0)
            {
                var today = DateTime.Now;

                var sortedBookings = selectedCustomer.ListOfBookingsInCustomer
                    .OrderBy(b => b.StartDate)
                    .ToList();

                var beforeToday = sortedBookings.Where(b => b.StartDate < today).TakeLast(3).ToList();
                var afterToday = sortedBookings.Where(b => b.StartDate >= today).Take(3).ToList();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  --- Kommande bokningar (3 närmsta) ---");
                foreach (var booking in afterToday)
                {
                    Console.WriteLine($"      Bokningnummer: {booking.BookingId}");
                    Console.WriteLine($"      Antal besökare: {booking.NumberOfGuests}");
                    Console.WriteLine($"      Incheckning: {booking.StartDate}");
                    Console.WriteLine($"      Utcheckning: {booking.EndDate}");
                    Console.WriteLine($"      Övrig info: {booking.OtherInfoInBooking}");
                    Console.WriteLine("      -");
                }
                Console.WriteLine("\n     --- Tidigare bokningar (3 senaste) ---");
                foreach (var booking in beforeToday)
                {
                    Console.WriteLine($"         Bokningnummer: {booking.BookingId}");
                    Console.WriteLine($"         Antal besökare: {booking.NumberOfGuests}");
                    Console.WriteLine($"         Incheckning: {booking.StartDate}");
                    Console.WriteLine($"         Utcheckning: {booking.EndDate}");
                    Console.WriteLine($"         Övrig info: {booking.OtherInfoInBooking}");
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
            _customerController.Value.MenuSwitch();
        }

        //public Customer GetCustomerNumber(Customer customer, bool isNew)
        //{
        //    string messageToUseInHeader = "Uppdatera kundnummer";
        //    if (isNew)
        //        messageToUseInHeader = "Skapa nytt kundnummer";

        //    while (true)
        //    {
        //        Console.Clear();
        //        Graphics.ShowMainGraphics();
        //        Console.ForegroundColor = ConsoleColor.Blue;
        //        Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
        //        Console.ResetColor();
        //        Messages.RequiredInputMessage();
        //        Console.WriteLine("   1. Det måste vara ett possitivt heltal.\n   2. Det måste vara ett unikt kundnummer.");
        //        if (!isNew)
        //        {
        //            Console.Write("  Nuvarande kundnummer: ");
        //            Console.ForegroundColor = ConsoleColor.Magenta;
        //            Console.WriteLine(customer.CustomerId);
        //            Console.ResetColor();
        //        }
        //        Messages.SetValueWithCursor();

        //        string inputCustomerNumber = Console.ReadLine();

        //        if (inputCustomerNumber.ToLower() == "exit")
        //            return customer;

        //        if (short.TryParse(inputCustomerNumber, out short customerNumber))
        //        {
        //            if (customerNumber > 0)
        //            {
        //                if (!_dbContext.Customers.Any(c => c.CustomerId == customerNumber))
        //                {
        //                    customer.CustomerId = customerNumber;
        //                    Messages.SuccessfullInput();
        //                    return customer;
        //                }
        //                else
        //                    Console.WriteLine("\n  Kundnummret är redan upptaget. Försök igen.");

        //            }
        //            else
        //                Console.WriteLine("\n  Kundnummret måste vara mer än 0. Försök igen.");
        //        }
        //        else
        //            Console.WriteLine("\n  Ogiltigt kundnummer. Vänligen ange ett nummer.");

        //        Console.WriteLine("\n  Tryck valfri tangent för att försöka igen...");
        //        Console.ReadKey();
        //    }
        //}

        public Customer GetFirstName(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera förnamn på kund";
            if (isNew)
                messageToUseInHeader = $"Förnamn Kund";

            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();
            Messages.RequiredInputMessage();
            Console.WriteLine("  Inga krav finns.");

            if (!isNew)
            {
                Console.WriteLine($"  Nuvarande förnamn:\n  {customer.FirstName}\n\n  Uppdatera förnamnet på kund:");
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(31, currentLineCursor - 1);
            }
            else
                Messages.SetValueWithCursor();

            string? firstNameInput = Console.ReadLine();
            if (firstNameInput.ToLower() == "exit")
                return customer;
            customer.FirstName = firstNameInput;
            Messages.SuccessfullInput();
            return customer;
        }

        public Customer GetLastName(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera efternamn på kund";
            if (isNew)
                messageToUseInHeader = $"Efternamn Kund";

            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();
            Messages.RequiredInputMessage();
            Console.WriteLine("  Inga krav finns.");

            if (!isNew)
            {
                Console.WriteLine($"  Nuvarande efternamn:\n  {customer.LastName}\n\n  Uppdatera efternamnet på kund:");
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(33, currentLineCursor - 1);
            }
            else
                Messages.SetValueWithCursor();

            string? lastNameInput = Console.ReadLine();
            if (lastNameInput.ToLower() == "exit")
                return customer;
            customer.LastName = lastNameInput;
            Messages.SuccessfullInput();
            return customer;
        }

        public Customer GetAddress(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera en kunds adress";
            if (isNew)
                messageToUseInHeader = $"Adress Kund";

            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
            Console.ResetColor();
            Messages.RequiredInputMessage();
            Console.WriteLine("  Inga krav finns.");

            if (!isNew)
            {
                Console.WriteLine($"  Nuvarande adress:\n  {customer.Address}\n\n  Uppdatera en kunds adress:");
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(29, currentLineCursor - 1);
            }
            else
                Messages.SetValueWithCursor();

            string? addressInput = Console.ReadLine();
            if (addressInput.ToLower() == "exit")
                return customer;
            customer.Address = addressInput;
            Messages.SuccessfullInput();
            return customer;
        }

        public Customer GetPhoneNumber(Customer customer, bool isNew)
        {
            string messageToUseInHeader = isNew ? $"Telefonnummer Kund" : $"Uppdatera en kunds telefonnummer";

            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
                Console.ResetColor();

                Messages.RequiredInputMessage();
                Console.WriteLine("  Ange ett giltigt telefonnummer (endast siffror och tillåtna symboler).");

                if (!isNew)
                {
                    Console.WriteLine($"  Nuvarande telnr:\n  {customer.PhoneNumber}\n\n  Uppdatera en kunds telnr:");
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(28, currentLineCursor - 1);
                }
                else
                    Messages.SetValueWithCursor();

                string? phoneNumberInput = Console.ReadLine();

                if (phoneNumberInput.ToLower() == "exit")
                    return customer;

                // Kontrollera om telefonnumret är giltigt
                if (IsValidPhoneNumber(phoneNumberInput))
                {
                    customer.PhoneNumber = phoneNumberInput;
                    Messages.SuccessfullInput();
                    return customer;
                }
                else
                {
                    Console.WriteLine("\n  Ogiltigt telefonnummer. Försök igen.");
                    Console.WriteLine("  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Kontrollera om null eller tomt
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Kontrollera om telefonnumret innehåller otillåtna tecken
            string validCharactersPattern = @"^[0-9\s\-()+]*$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, validCharactersPattern))
                return false;

            // Kontrollera längden
            if (phoneNumber.Length < 5 || phoneNumber.Length > 15)
                return false;

            return true;
        }

        public Customer GetEmail(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera en kunds Epost";
            if (isNew)
                messageToUseInHeader = $"Epost Kund";
            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
                Console.ResetColor();
                Messages.RequiredInputMessage();
                Console.WriteLine("  Inga krav finns.");

                if (!isNew)
                {
                    Console.WriteLine($"  Nuvarande Epost:\n  {customer.EmailAddress}\n\n  Uppdatera en kunds Epost:");
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(28, currentLineCursor - 1);
                }
                else
                    Messages.SetValueWithCursor();

                string? emailAddressInput = Console.ReadLine();
                if (emailAddressInput.ToLower() == "exit")
                    return customer;
                if (string.IsNullOrWhiteSpace(emailAddressInput))
                {
                    Console.WriteLine("\n  E-postadressen får inte vara tom. Försök igen.");
                    Console.WriteLine("  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                    continue;
                }
                if (IsValidEmail(emailAddressInput))
                {
                    customer.EmailAddress = emailAddressInput;
                    Messages.SuccessfullInput();
                    return customer;
                }
                else
                    Console.WriteLine("\n  Ogiltig e-postadress. Försök igen.");
                    
                Console.WriteLine("  Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
            
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public Customer GetMembership(Customer customer, bool isNew)
        {
            List<string> listOfMemberships = new List<string>
            {
            "Bronze", "Silver", "Gold"
            };
            string messageToUseInHeader = $"Välj medlemsnivå ↑/↓/↩ - Nuvarande nivå: {customer.Membership}";
            if (isNew)
                messageToUseInHeader = $"Välj medlemsnivå ↑/↓/↩ - Standardvärde: {customer.Membership}";

            var selectedMembership = _displayList.BrowseAList(listOfMemberships, false, Graphics.GetHeaderAsString(messageToUseInHeader), false);

            if (selectedMembership == -1)
                return customer;
            else if (selectedMembership == 0)
            {
                customer.Membership = TypeOfMembership.Brons;
                Messages.SuccessfullInput();
                return customer;
            }
            else if (selectedMembership == 1)
            {
                customer.Membership = TypeOfMembership.Silver;
                Messages.SuccessfullInput();
                return customer;
            }
            else if (selectedMembership == 2)
            {
                customer.Membership = TypeOfMembership.Guld;
                Messages.SuccessfullInput();
                return customer;
            }
            else
                return customer;
        }

        public Customer GetDateOfBirth(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera en kunds födelsedatum";
            if (isNew)
                messageToUseInHeader = $"Födelsedatum Kund";

            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString(messageToUseInHeader));
                Console.ResetColor();
                Messages.RequiredInputMessage();
                Console.WriteLine("  1. Datumet måste vara i formatet: YYYY-MM-DD");

                if (!isNew)
                {
                    Console.WriteLine($"  Nuvarande födelsedatum:\n  {customer.DateOfBirth}\n\n  Uppdatera en kunds födelsedatum:");
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(35, currentLineCursor - 1);
                }
                    
                else
                    Messages.SetValueWithCursor();

                string? dateOfBirthInput = Console.ReadLine();
                if (dateOfBirthInput.ToLower() == "exit")
                    return customer;
                if (DateOnly.TryParseExact(dateOfBirthInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateOnly dateOfBirth))
                {
                    // Kontrollera om datumet ligger inom det tillåtna intervallet
                    if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now))
                        Console.WriteLine("  Födelsedatumet kan inte vara i framtiden.");
                    else
                    {
                        customer.DateOfBirth = dateOfBirth;
                        Messages.SuccessfullInput();
                        return customer;
                    }
                }
                else
                    Console.WriteLine("  Ogiltigt format. Ange datum i formatet YYYY-MM-DD.");

                Console.WriteLine("\n  Tryck valfri tangent för att försöka igen...");
                Console.ReadKey();
            }
        }

        public Customer GetOtherInfo(Customer customer, bool isNew)
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
                Console.WriteLine($"  Nuvarande uppgifter:\n  {customer.OtherInfoInCustomer}\n\n  Uppdatera övriga uppgifter (valfritt):");
            else
                Console.WriteLine("  Ange övriga uppgifter (valfritt): ");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor + 0);
            string? otherInput = Console.ReadLine();
            customer.OtherInfoInCustomer = otherInput;
            Messages.SuccessfullInput();
            return customer;
        }

        public bool ValidateCustomer(Customer customer, bool isNew)
        {
            if (customer.FirstName == "undefined" || customer.LastName == "undefined" || customer.EmailAddress == "undefined" || customer.PhoneNumber == "undefined")
                return false;
            if (_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId) && isNew == true)
            {
                Console.WriteLine("\n  Ett kund med detta kundnummer finns redan.\n  Tryck på valfri tangent för att försöka igen...");
                Console.ReadKey();
                return false;
            }
            return true;
        }

        public void SaveCustomerToDataBase(Customer customer)
        {
            _dbContext.Customers.Add(customer);

            Console.Write("\n  Kund ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(customer.CustomerId);
            Console.ResetColor();
            Console.WriteLine(" har sparats.");
            Thread.Sleep(1000);
            ReadOneCustomer(customer);
        }

        public void DeactivateACustomer(Customer customer)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString("Avaktivera / Aktivera kund"));
            Console.ResetColor();

            if (customer.IsActive == true)
                customer.IsActive = false;
            else
                customer.IsActive = true;
            Console.Write("\n  Följande kund är ändrat: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(customer.CustomerId);
            Console.ResetColor();

            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
            _customerController.Value.MenuSwitch();
        }
        public Customer GetCustomer()
        {
            List<string> listOfChoice = new List<string>
            {
            "Ny kund", "Sök befintlig kund"
            };

            var selectedCustomerChoice = _displayList.BrowseAList(listOfChoice, false, Graphics.GetHeaderAsString("Välj ny eller befintlig kund"), false);

            if (selectedCustomerChoice == -1)
            {
                AbortInCustomerService();
            }
            else if (selectedCustomerChoice == 0)
            {
                // Skapa ny kund och returnera den
                var blankCustomer = new Customer
                {
                    FirstName = "undefined",
                    LastName = "undefined",
                    PhoneNumber = "undefined",
                    EmailAddress = "undefined"
                };

                var createdCustomer = _customerPropertySelector.Value.PropertySwitch(blankCustomer, false, true);
                return createdCustomer;
            }
            else if (selectedCustomerChoice == 1)
            {
                // Hitta befintlig kund och returnera den
                var foundCustomer = GetACustomerInList(
                    GetListOfCustomersBySearch(false, false), false, false
                );
                return foundCustomer;
            }
            else
            {
                Console.WriteLine("  Fel i val av kund. Avbryter processen.\n  Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
                AbortInCustomerService();
            }

            // Fallback: Detta behövs för att undvika kompileringsfel
            return null;
        }
        //public Customer GetCustomer()
        //{
        //    List<string> listOfChoice = new List<string>
        //    {
        //    "Ny kund", "Befintlig kund"
        //    };

        //    var selectedCustomerChoice = _displayList.BrowseAList(listOfChoice, false, Graphics.GetHeaderAsString("Välj ny eller befintlig kund"), false);
        //    var blankCustomer = new Customer { FirstName = "undefined", LastName = "undefined", PhoneNumber = "undefined", EmailAddress = "undefined" };
        //    if (selectedCustomerChoice == -1)
        //        AbortInCustomerService();
        //    else if (selectedCustomerChoice == 0)
        //    {
        //        var createdCustomer = _customerPropertySelector.Value.PropertySwitch(blankCustomer, false, true); //Denna metod har returtyp void. Den skapar en kund och lägger in den i databas med dbContext.
        //        return createdCustomer;
        //    }
        //    else if (selectedCustomerChoice == 1)
        //    {
        //        var foundCustomer = _customerService.GetACustomerInList(_customerService.GetListOfCustomersBySearch(false, false), false, false));
        //        return foundCustomer;
        //    }
        //    else
        //    {
        //        Console.WriteLine("  Fel i val av nu kund, avbryter bokning.\n  Tryck på valfri tangent för att fortsätta...");
        //        Console.ReadKey();
        //        AbortInCustomerService();
        //    }
        //    return blankCustomer;
        //}

        public void AbortInCustomerService()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n  Avbryter bokning...");
            Console.ResetColor();
            Thread.Sleep(1000);
            _mainMenu.Value.MenuSwitch();
        }
    }
}
