using HotelApp.Controllers;
using HotelApp.UI;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.UI.Menus
{
    /// <summary>
    /// Klassen hanterar undermenyn ServiceMenu
    /// </summary>
    public class ServiceMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly IMenu _mainMenu;
        private readonly Lazy<BookingController> _bookingMenu;
        private readonly Lazy<CustomerController> _customerMenu;
        private readonly Lazy<RoomController> _roomMenu;
        private readonly Lazy<InvoiceController> _invoiceMenu;

        public ServiceMenu(IMenu mainMenu, DisplayList displayList, Lazy<BookingController> bookingMenu, Lazy<CustomerController> customerMenu, Lazy<RoomController> roomMenu, Lazy<InvoiceController> invoiceMenu)
        {
            _displayList = displayList;
            _mainMenu = mainMenu;
            _bookingMenu = bookingMenu;
            _customerMenu = customerMenu;
            _roomMenu = roomMenu;
            _invoiceMenu = invoiceMenu;
        }
        public void MenuSwitch()
        {
            List<string> listServiceMenu = new List<string>
            {
                "Bokningar", "Kunder", "Fakturor", "Rum"
            };
            switch (_displayList.BrowseAList(listServiceMenu, false, Graphics.GetHeaderAsString("Hanteringsmeny ↑/↓/↩"), true))
            {
                case 0:
                    _bookingMenu.Value.MenuSwitch();
                    break;
                case 1:
                    _customerMenu.Value.MenuSwitch();
                    break;
                case 2:
                    _invoiceMenu.Value.MenuSwitch();
                    break;
                case 3:
                    _roomMenu.Value.MenuSwitch();
                    break;
                case 4:
                    _mainMenu.MenuSwitch();
                    return;
                default:
                    Console.WriteLine("Ogiltigt alternativ 'ServiceMenu', tryck valfri tangent för att återgå.");
                    Console.ReadKey();
                    _mainMenu.MenuSwitch();
                    break;
            }
        }
    }
}
