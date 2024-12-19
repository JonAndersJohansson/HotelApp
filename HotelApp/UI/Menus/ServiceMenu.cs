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
        private readonly Lazy<BookingController> _bookingController;
        private readonly Lazy<CustomerController> _customerController;
        private readonly Lazy<RoomController> _roomController;
        private readonly Lazy<InvoiceController> _invoiceController;

        public ServiceMenu(IMenu mainMenu, DisplayList displayList, Lazy<BookingController> bookingMenu, Lazy<CustomerController> customerMenu, Lazy<RoomController> roomMenu, Lazy<InvoiceController> invoiceMenu)
        {
            _displayList = displayList;
            _mainMenu = mainMenu;
            _bookingController = bookingMenu;
            _customerController = customerMenu;
            _roomController = roomMenu;
            _invoiceController = invoiceMenu;
        }
        public void MenuSwitch()
        {
            List<string> listServiceMenu = new List<string>
            {
                "Bokningar", "Kunder", "Fakturor", "Rum"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(listServiceMenu, false, Graphics.GetHeaderAsString("Hanteringsmeny ↑/↓/↩"), true))
                {
                    case 0:
                        _bookingController.Value.MenuSwitch();
                        break;
                    case 1:
                        _customerController.Value.MenuSwitch();
                        break;
                    case 2:
                        _invoiceController.Value.MenuSwitch();
                        break;
                    case 3:
                        _roomController.Value.MenuSwitch();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ 'ServiceMenu', tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        return;
                }
            }
            
        }
    }
}
