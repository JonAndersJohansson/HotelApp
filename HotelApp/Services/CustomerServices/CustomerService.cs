using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.Services.BookingService;
using HotelApp.UI;
using HotelApp.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Services.CustomerServices
{
    public class CustomerService
    {
        private readonly DisplayList _displayList;
        private ApplicationDbContext _dbContext;
        private readonly Lazy<CustomerPropertySelector> _customerPropertySelector;
        public CustomerService(DisplayList displayList, ApplicationDbContext 
            dbContext, Lazy<CustomerPropertySelector> customerPropertySelector)
        {
            _displayList = displayList;
            _dbContext = dbContext;
            _customerPropertySelector = customerPropertySelector;
        }
        public void SearchCustomerToList(bool isDeactivate, bool isToChange)
        {
            string messageToUseInHeader = "Sök kund";
            if (isDeactivate)
                messageToUseInHeader = "Sök för att Aktivera/Avaktivera kund";
            if (isToChange)
                messageToUseInHeader = "Sök för att ändra kunduppgifter";
            Messages.ClearAndShowHeader(messageToUseInHeader);
            Messages.RequiredInputMessage();
            Console.WriteLine("   1. Sökbar kundinfo: Namn, KundNr, Telnr, Epost\n  Sök:");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(7, currentLineCursor - 1);
            string? userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("  Inga kunder hittades som matchar din " +
                    "sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
                return;
            }
            if (userInput.ToLower() == "exit")
                return;

            userInput = userInput.ToLower();
            var matchingCustomers = _dbContext.Customers
                .Where(c =>
                    c.FirstName.ToLower().Contains(userInput) ||
                    c.LastName.ToLower().Contains(userInput) ||
                    c.PhoneNumber.ToLower().Contains(userInput) ||
                    c.EmailAddress.ToLower().Contains(userInput) ||
                    c.Id.ToString().Contains(userInput))
                .ToList();

            if (!matchingCustomers.Any())
            {
                Console.WriteLine("  Inga kunder hittades som matchar din " +
                    "sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
            }
            SelectCustomerFromList(matchingCustomers, isDeactivate, isToChange);
            return;
        }
        public void SelectCustomerFromList(List<Customer> matchingCustomers, 
            bool isDeactivate, bool isToChange)
        {
            string messageToUseInHeader = "Sökresultat, välj kund för att " +
                "visa all info ↑/↓/↩";
            if (isDeactivate)
                messageToUseInHeader = "Sökresultat, välj kund för att " +
                    "Avaktivera / Aktivera ↑/↓/↩";
            if (isToChange)
                messageToUseInHeader = "Sökresultat, välj kund att ändra ↑/↓/↩";

            var selectedIndex = _displayList.BrowseAList(matchingCustomers, 
                false, Graphics.GetHeaderAsString(messageToUseInHeader), false);
            if (selectedIndex >= 0 && selectedIndex < matchingCustomers.Count 
                && isDeactivate && !isToChange)
                DeactivateACustomer(matchingCustomers[selectedIndex]);
            else if (selectedIndex >= 0 && selectedIndex < matchingCustomers.
                Count && isToChange && !isDeactivate)
                _customerPropertySelector.Value.PropertySwitch(
                    matchingCustomers[selectedIndex], false, false);
            else if (selectedIndex >= 0 && selectedIndex < matchingCustomers.Count 
                && !isDeactivate && !isToChange)
                ReadOneCustomer(matchingCustomers[selectedIndex]);
            else if (selectedIndex == -1)
                return;
            else
            {
                Console.WriteLine("  Fel: Ogiltigt val c SelectCustomerFromList." +
                    "\n  Tryck valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
        }
        public void ReadOneCustomer(Customer selectedCustomer)
        {
            _dbContext.Entry(selectedCustomer)
                .Collection(c => c.ListOfBookingsInCustomer)
                .Load();

            Messages.ClearAndShowHeader($"  Info kundnummer: " +
                $"{selectedCustomer.Id}");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  Kundnummer: {selectedCustomer.Id}");
            Console.WriteLine($"  Namn: {selectedCustomer.LastName}, " +
                $"{selectedCustomer.FirstName}");
            Console.WriteLine($"  Adress: {selectedCustomer.Address}");
            Console.WriteLine($"  Telefon nr: {selectedCustomer.PhoneNumber}");
            Console.WriteLine($"  Email: {selectedCustomer.EmailAddress}");
            Console.WriteLine($"  Kundnivå: {selectedCustomer.Membership}");
            Console.WriteLine($"  Födelsedatum: {selectedCustomer.DateOfBirth}");
            Console.WriteLine($"  Övrig info: {selectedCustomer.OtherInfoInCustomer}");
            Console.ResetColor();

            if (selectedCustomer.ListOfBookingsInCustomer?.Count > 0)
            {
                var today = DateTime.Now;

                var sortedBookings = selectedCustomer.ListOfBookingsInCustomer
                    .OrderBy(b => b.StartDate)
                    .ToList();

                var beforeToday = sortedBookings
                    .Where(b => b.StartDate < today)
                    .TakeLast(3)
                    .ToList();
                var afterToday = sortedBookings
                    .Where(b => b.StartDate >= today)
                    .Take(3)
                    .ToList();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  --- Kommande bokningar (3 närmsta) ---");
                foreach (var booking in afterToday)
                {
                    Console.WriteLine($"      Bokningnummer: {booking.Id}");
                    Console.WriteLine($"      Antal besökare: " +
                        $"{booking.NumberOfGuests}");
                    Console.WriteLine($"      Incheckning: " +
                        $"{booking.StartDate}");
                    Console.WriteLine($"      Utcheckning: " +
                        $"{booking.EndDate}");
                    Console.WriteLine($"      Övrig info: " +
                        $"{booking.OtherInfoInBooking}");
                    Console.WriteLine("      -");
                }
                Console.WriteLine("\n     --- Tidigare bokningar (3 senaste) ---");
                foreach (var booking in beforeToday)
                {
                    Console.WriteLine($"         Bokningnummer: {booking.Id}");
                    Console.WriteLine($"         Antal besökare: " +
                        $"{booking.NumberOfGuests}");
                    Console.WriteLine($"         Incheckning: " +
                        $"{booking.StartDate}");
                    Console.WriteLine($"         Utcheckning: " +
                        $"{booking.EndDate}");
                    Console.WriteLine($"         Övrig info: " +
                        $"{booking.OtherInfoInBooking}");
                    Console.WriteLine("         -");
                }
                Console.ResetColor();
            }
            else
                Console.WriteLine("\n  Inga relaterade bokningar.");
            
            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }
        public Customer GetFirstName(Customer customer, bool isNew)
        {
            string messageToUseInHeader = isNew ? "Förnamn Kund" : "Uppdatera förnamn på kund";

            while (true)
            {
                Messages.ClearAndShowHeader(messageToUseInHeader);
                Messages.RequiredInputMessage();
                Console.WriteLine("  Endast bokstäver är tillåtna.");

                if (!isNew)
                {
                    Console.WriteLine($"  Nuvarande förnamn:\n  {customer.FirstName}\n\n  Uppdatera förnamnet på kund:");
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(31, currentLineCursor - 1);
                }
                else
                {
                    Messages.SetValueWithCursor();
                }

                string? firstNameInput = Console.ReadLine();

                if (firstNameInput?.ToLower() == "exit")
                    return customer;

                if (!string.IsNullOrWhiteSpace(firstNameInput) && System.Text.RegularExpressions.Regex.IsMatch(firstNameInput, @"^[A-Za-zÅÄÖåäö]+$"))
                {
                    customer.FirstName = firstNameInput;
                    Messages.SuccessfullInputSave();
                    return customer;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Ogiltigt förnamn. Endast bokstäver är tillåtna. Försök igen.");
                    Console.ResetColor();
                    Console.WriteLine("\n  Tryck på valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
            }
        }

        public Customer GetLastName(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera efternamn på kund";
            if (isNew)
                messageToUseInHeader = $"Efternamn Kund";

            Messages.ClearAndShowHeader(messageToUseInHeader);
            Messages.RequiredInputMessage();
            Console.WriteLine("  Inga krav finns.");

            if (!isNew)
            {
                Console.WriteLine($"  Nuvarande efternamn:\n  " +
                    $"{customer.LastName}\n\n  Uppdatera efternamnet på kund:");
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(33, currentLineCursor - 1);
            }
            else
                Messages.SetValueWithCursor();

            string? lastNameInput = Console.ReadLine();
            if (lastNameInput?.ToLower() == "exit")
                return customer;
            customer.LastName = lastNameInput;
            Messages.SuccessfullInputSave();
            return customer;
        }
        public Customer GetAddress(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera en kunds adress";
            if (isNew)
                messageToUseInHeader = $"Adress Kund";

            Messages.ClearAndShowHeader(messageToUseInHeader);
            Messages.RequiredInputMessage();
            Console.WriteLine("  Inga krav finns.");

            if (!isNew)
            {
                Console.WriteLine($"  Nuvarande adress:\n  {customer.Address}" +
                    $"\n\n  Uppdatera en kunds adress:");
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(29, currentLineCursor - 1);
            }
            else
                Messages.SetValueWithCursor();

            string? addressInput = Console.ReadLine();
            if (addressInput?.ToLower() == "exit")
                return customer;
            customer.Address = addressInput;
            Messages.SuccessfullInputSave();
            return customer;
        }
        public Customer GetPhoneNumber(Customer customer, bool isNew)
        {
            string messageToUseInHeader = isNew ? $"Telefonnummer Kund" : 
                $"Uppdatera en kunds telefonnummer";

            while (true)
            {
                Messages.ClearAndShowHeader(messageToUseInHeader);
                Messages.RequiredInputMessage();
                Console.WriteLine("  Ange ett giltigt telefonnummer " +
                    "(endast siffror och tillåtna symboler).");

                if (!isNew)
                {
                    Console.WriteLine($"  Nuvarande telnr:\n  " +
                        $"{customer.PhoneNumber}\n\n  Uppdatera en kunds telnr:");
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(28, currentLineCursor - 1);
                }
                else
                    Messages.SetValueWithCursor();

                string? phoneNumberInput = Console.ReadLine();

                if (phoneNumberInput?.ToLower() == "exit")
                    return customer;

                // Kontrollera om telefonnumret är giltigt
                if (IsValidPhoneNumber(phoneNumberInput))
                {
                    customer.PhoneNumber = phoneNumberInput;
                    Messages.SuccessfullInputSave();
                    return customer;
                }
                else
                {
                    Console.WriteLine("\n  Ogiltigt telefonnummer. " +
                        "Försök igen.");
                    Console.WriteLine("  Tryck på valfri tangent " +
                        "för att fortsätta...");
                    Console.ReadKey();
                }
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            string validCharactersPattern = @"^[0-9\s\-()+]*$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, 
                validCharactersPattern))
                return false;

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
                Messages.ClearAndShowHeader(messageToUseInHeader);
                Messages.RequiredInputMessage();
                Console.WriteLine("  Inga krav finns.");

                if (!isNew)
                {
                    Console.WriteLine($"  Nuvarande Epost:\n  " +
                        $"{customer.EmailAddress}\n\n  Uppdatera en kunds Epost:");
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(28, currentLineCursor - 1);
                }
                else
                    Messages.SetValueWithCursor();

                string? emailAddressInput = Console.ReadLine();
                if (emailAddressInput?.ToLower() == "exit")
                    return customer;
                if (string.IsNullOrWhiteSpace(emailAddressInput))
                {
                    Console.WriteLine("\n  E-postadressen får inte vara tom. " +
                        "Försök igen.");
                    Console.WriteLine("  Tryck på valfri tangent för att " +
                        "fortsätta...");
                    Console.ReadKey();
                    continue;
                }
                if (IsValidEmail(emailAddressInput))
                {
                    customer.EmailAddress = emailAddressInput;
                    Messages.SuccessfullInputSave();
                    return customer;
                }
                else
                    Console.WriteLine("\n  Ogiltig e-postadress. Försök igen.");
                    
                Console.WriteLine("  Tryck på valfri tangent för att " +
                    "fortsätta...");
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
            string messageToUseInHeader = $"Välj medlemsnivå ↑/↓/↩ - " +
                $"Nuvarande nivå: {customer.Membership}";
            if (isNew)
                messageToUseInHeader = $"Välj medlemsnivå ↑/↓/↩ - " +
                    $"Standardvärde: {customer.Membership}";

            var selectedMembership = _displayList.BrowseAList(listOfMemberships, 
                false, Graphics.GetHeaderAsString(messageToUseInHeader), false);

            if (selectedMembership == -1)
                return customer;
            else if (selectedMembership == 0)
            {
                customer.Membership = TypeOfMembership.Brons;
                Messages.SuccessfullInputSave();
                return customer;
            }
            else if (selectedMembership == 1)
            {
                customer.Membership = TypeOfMembership.Silver;
                Messages.SuccessfullInputSave();
                return customer;
            }
            else if (selectedMembership == 2)
            {
                customer.Membership = TypeOfMembership.Guld;
                Messages.SuccessfullInputSave();
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
                Messages.ClearAndShowHeader(messageToUseInHeader);
                Messages.RequiredInputMessage();
                Console.WriteLine("  1. Datumet måste vara i formatet: " +
                    "YYYY-MM-DD");

                if (!isNew)
                {
                    Console.WriteLine($"  Nuvarande födelsedatum:\n  " +
                        $"{customer.DateOfBirth}\n\n  Uppdatera en kunds " +
                        $"födelsedatum:");
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(35, currentLineCursor - 1);
                }
                else
                    Messages.SetValueWithCursor();

                string? dateOfBirthInput = Console.ReadLine();
                if (dateOfBirthInput?.ToLower() == "exit")
                    return customer;
                if (DateOnly.TryParseExact(dateOfBirthInput, "yyyy-MM-dd", 
                    null, System.Globalization.DateTimeStyles.None, out 
                    DateOnly dateOfBirth))
                {
                    if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now))
                        Console.WriteLine("  Födelsedatumet kan inte vara i " +
                            "framtiden.");
                    else
                    {
                        customer.DateOfBirth = dateOfBirth;
                        Messages.SuccessfullInputSave();
                        return customer;
                    }
                }
                else
                    Console.WriteLine("  Ogiltigt format. Ange datum i " +
                        "formatet YYYY-MM-DD.");

                Console.WriteLine("\n  Tryck valfri tangent för att försöka " +
                    "igen...");
                Console.ReadKey();
            }
        }
        public Customer GetOtherInfo(Customer customer, bool isNew)
        {
            string messageToUseInHeader = $"Uppdatera övriga uppgifter";
            if (isNew)
                messageToUseInHeader = $"Övriga uppgifter";

            Messages.ClearAndShowHeader(messageToUseInHeader);
            if (!isNew)
                Console.WriteLine($"  Nuvarande uppgifter:\n  " +
                    $"{customer.OtherInfoInCustomer}\n\n  " +
                    $"Uppdatera övriga uppgifter (valfritt):");
            else
                Console.WriteLine("  Ange övriga uppgifter (valfritt): ");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(2, currentLineCursor + 0);
            string? otherInput = Console.ReadLine();
            customer.OtherInfoInCustomer = otherInput;
            Messages.SuccessfullInputSave();
            return customer;
        }
        public bool ValidateCustomer(Customer customer, bool isNew)
        {
            if (customer.FirstName == "undefined" || customer.LastName == 
                "undefined" || customer.EmailAddress == "undefined" || 
                customer.PhoneNumber == "undefined")
                return false;

            return true;
        }
        public void SaveCustomerToDataBase(Customer customer)
        {
            var entry = _dbContext.Entry(customer);
            if (entry.State == EntityState.Detached)
                _dbContext.Customers.Add(customer);

            _dbContext.SaveChanges();

            Console.Write("\n  Kund ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(customer.Id);
            Console.ResetColor();
            Console.WriteLine(" har sparats.");
            Thread.Sleep(1000);

            ReadOneCustomer(customer);
        }
        public void DeactivateACustomer(Customer customer)
        {
            Messages.ClearAndShowHeader("Avaktivera / Aktivera kund");

            var hasActiveBookings = _dbContext.Bookings
                .Any(b => b.CustomerId == customer.Id && !b.IsCancelled);

            if (hasActiveBookings)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Kunden kan inte avaktiveras eftersom det finns aktiva bokningar kopplade till denna kund.");
                Console.ResetColor();
            }
            else
            {
                customer.IsActive = false;

                var entry = _dbContext.Entry(customer);
                if (entry.State == EntityState.Detached)
                    _dbContext.Customers.Attach(customer);

                _dbContext.SaveChanges();

                Console.Write("\n  Följande kund är ändrat: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"  KundId: {customer.Id}, Aktiv status: {(customer.IsActive ? "Aktiv" : "Inaktiv")}");
                Console.ResetColor();
            }
            Console.WriteLine("\n  Tryck på någon tangent för att återgå...");
            Console.ReadKey();
        }
        public Customer? GetCustomer()
        {
            List<string> listOfChoice = new List<string>
            {
            "Ny kund", "Sök befintlig kund"
            };

            var selectedCustomerChoice = _displayList.BrowseAList(listOfChoice, 
                false, Graphics.GetHeaderAsString("Välj ny eller befintlig kund"), false);

            if (selectedCustomerChoice == -1)
                return null;

            else if (selectedCustomerChoice == 0)
            {
                var blankCustomer = new Customer
                {
                    FirstName = "undefined",
                    LastName = "undefined",
                    PhoneNumber = "undefined",
                    EmailAddress = "undefined"
                };
                var createdCustomer = _customerPropertySelector.Value.
                    PropertySwitch(blankCustomer, true, true);
                return createdCustomer;
            }
            else if (selectedCustomerChoice == 1)
            {
                return GetCustomerBySearch();
            }
            else
            {
                Console.WriteLine("  Fel: Ogiltigt värde GetCustomer. " +
                    "Avbryter processen.\n  Tryck på valfri tangent för att " +
                    "fortsätta...");
                Console.ReadKey();
            }
            return null;
        }
        public Customer? GetCustomerBySearch()
        {
            Messages.ClearAndShowHeader("Sök kund");
            Messages.RequiredInputMessage();
            Console.WriteLine("   1. Sökbar kundinfo: Namn, KundNr, Telnr, " +
                "Epost");
            Console.WriteLine("\n  Sök:");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(7, currentLineCursor - 1);
            string? userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("  Inga kunder hittades som matchar din " +
                    "sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
                return null;
            }
            if (userInput.ToLower() == "exit")
                return null;

            userInput = userInput.ToLower();
            var matchingCustomers = _dbContext.Customers
                .Where(c =>
                    c.FirstName.ToLower().Contains(userInput) ||
                    c.LastName.ToLower().Contains(userInput) ||
                    c.PhoneNumber.ToLower().Contains(userInput) ||
                    c.EmailAddress.ToLower().Contains(userInput) ||
                    c.Id.ToString().Contains(userInput))
                .ToList();

            if (!matchingCustomers.Any())
            {
                Console.WriteLine("  Inga kunder hittades som matchar din " +
                    "sökning.\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
            }
            return GetCustomerInSearch(matchingCustomers);
        }
        private Customer? GetCustomerInSearch(List<Customer> matchingCustomers)
        {
            var selectedIndex = _displayList.BrowseAList(matchingCustomers, 
                false, Graphics.GetHeaderAsString("Sökresultat, välj kund för " +
                "bokning"), false);
            if (selectedIndex >= 0 && selectedIndex < matchingCustomers.Count)
                return matchingCustomers[selectedIndex];
            else if (selectedIndex == -1)
                return null;
            else
            {
                Console.WriteLine("  Fel: Ogiltigt värde c GetCustomerInSearch." +
                    "\n  Tryck valfri tangent för att återgå...");
                Console.ReadKey();
                return null;
            }
        }
    }
}
