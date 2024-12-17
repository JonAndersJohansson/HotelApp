using HotelApp.Data.Models;

using HotelApp.Services;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.RoomServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Controllers
{
    /// <summary>
    /// Klassen hanterar undermenyn CustomerMenu
    /// </summary>
    public class CustomerController : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;
        private readonly CustomerService _customerService;
        private readonly Lazy<CustomerPropertySelector> _customerPropertySelector;

        public CustomerController(Lazy<ServiceMenu> serviceMenu, DisplayList displayList, CustomerService customerService, Lazy<CustomerPropertySelector> customerPropertySelector)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
            _customerService = customerService;
            _customerPropertySelector = customerPropertySelector;
        }
        public void MenuSwitch()
        {
            List<string> listCustomerMenu = new List<string>
            {
                "Sök & visa en kund", "Lägg till en ny kund", "Sök & ändra kundinformation på befintlig kund", "Sök & ta bort en kund (OM INGA BOKNINGAR FINNS KOPPLADE!!!)"
            };
            switch (_displayList.BrowseAList(listCustomerMenu, false, Graphics.GetHeaderAsString("Meny Kunder ↑/↓/↩"), true))
            {
                case 0:
                    _customerService.ReadOneCustomer(_customerService.GetACustomerInList(_customerService.GetListOfCustomersBySearch(false, false), false, false));
                    break;
                case 1:
                    var newCustomer = new Customer { FirstName = "undefined", LastName = "undefined", PhoneNumber = "undefined", EmailAddress = "undefined" };
                    _customerPropertySelector.Value.PropertySwitch(newCustomer, true, false);
                    break;
                case 2:
                    _customerPropertySelector.Value.PropertySwitch(_customerService.GetACustomerInList(_customerService.GetListOfCustomersBySearch(false, true), false, true), false, false);
                    break;
                case 3:
                    _customerService.DeactivateACustomer(_customerService.GetACustomerInList(_customerService.GetListOfCustomersBySearch(true, false),true, false));
                    break;
                case 4:
                    _serviceMenu.Value.MenuSwitch();
                    return;
                default:
                    Console.WriteLine("Ogiltigt alternativ 'CustomerMenu', tryck valfri tangent för att återgå.");
                    Console.ReadKey();
                    _serviceMenu.Value.MenuSwitch();
                    break;
            }
        }
    }
}
