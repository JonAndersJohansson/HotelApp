using HotelApp.Controllers;
using HotelApp.Data.Models;
using HotelApp.UI;
using HotelApp.Utilities;

namespace HotelApp.Services.CustomerServices
{
    public class CustomerPropertySelector
    {
        private readonly CustomerService _customerService;
        private readonly DisplayList _displayList;
        private readonly Lazy<CustomerController> _customerController;
        public CustomerPropertySelector(Lazy<CustomerController> customerController, DisplayList displayList, CustomerService customerService)
        {
            _customerController = customerController;
            _displayList = displayList;
            _customerService = customerService;
        }
        public Customer? PropertySwitch(Customer customer, bool isNewFromCustomerMenu, bool isNewFromBooking)
        {
            List<string> menuListInCustomerPropertySelector = new List<string>
            {
                "Förnamn *", "Efternamn *", "Adress", "Telefon nr *", "Email *", "KundNivå", "Födelsedatum", "Övrig information", "Kontrollera & Spara"
            };
            while (true)
            {
                string messageToUseInHeader = "Ny kund. Välj i listan och lägg till kundinformation ↑/↓/↩ (* = Krav)";
                if (customer.Id > 0)
                    messageToUseInHeader = $"Ändra {customer.FirstName} {customer.LastName}. Välj i listan för att ändra ↑/↓/↩ (* = Krav)";

                switch (_displayList.BrowseAList(menuListInCustomerPropertySelector, false, Graphics.GetHeaderAsString(messageToUseInHeader), true))
                {
                    case 0:
                        _customerService.GetFirstName(customer, isNewFromCustomerMenu);
                        break;
                    case 1:
                        _customerService.GetLastName(customer, isNewFromCustomerMenu);
                        break;
                    case 2:
                        _customerService.GetAddress(customer, isNewFromCustomerMenu);
                        break;
                    case 3:
                        _customerService.GetPhoneNumber(customer, isNewFromCustomerMenu);
                        break;
                    case 4:
                        _customerService.GetEmail(customer, isNewFromCustomerMenu);
                        break;
                    case 5:
                        _customerService.GetMembership(customer, isNewFromCustomerMenu);
                        break;
                    case 6:
                        _customerService.GetDateOfBirth(customer, isNewFromCustomerMenu);
                        break;
                    case 7:
                        _customerService.GetOtherInfo(customer, isNewFromCustomerMenu);
                        break;
                    case 8:
                        if (_customerService.ValidateCustomer(customer, isNewFromCustomerMenu) == true)
                        {
                            if (isNewFromBooking)
                            {
                                _customerService.SaveCustomerToDataBase(customer);
                                return customer;
                            }
                            _customerService.SaveCustomerToDataBase(customer);
                            _customerController.Value.MenuSwitch();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n  Ogiltigt värde, var vänlig fyll i alla obligatoriska uppgifter.\n  Tryck valfri tangent för att försöka igen...");
                            Console.ReadKey();
                            break;
                        }
                    case 9:
                        return null;
                    default:
                        Console.WriteLine("Ogiltigt alternativ i CustomerPropertySelector switch, tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        _customerController.Value.MenuSwitch();
                        break;
                }
            }
        }
    }
}
