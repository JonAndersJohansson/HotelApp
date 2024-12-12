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
        private readonly Lazy<MainMenu> _mainMenu;
        private readonly Lazy<BookingMenu> _bookingMenu;
        private readonly Lazy<CustomerMenu> _customerMenu;
        private readonly Lazy<RoomMenu> _roomMenu;
        private readonly Lazy<InvoiceMenu> _invoiceMenu;

        public ServiceMenu(Lazy<MainMenu> mainMenu, DisplayList displayList, Lazy<BookingMenu> bookingMenu, Lazy<CustomerMenu> customerMenu, Lazy<RoomMenu> roomMenu, Lazy<InvoiceMenu> invoiceMenu)
        {
            _displayList = displayList;
            _mainMenu = mainMenu;
            _bookingMenu = bookingMenu;
            _customerMenu = customerMenu;
            _roomMenu = roomMenu;
            _invoiceMenu = invoiceMenu;
        }

        public readonly List<string> listServiceMenu = new List<string>
        {
        "Bokningar", "Kunder", "Fakturor", "Rum"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
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
                    _mainMenu.Value.MenuSwitch();
                    return;
                default:
                    Console.WriteLine("Ogiltigt alternativ 'ServiceMenu', tryck valfri tangent för att återgå.");
                    Console.ReadKey();
                    _mainMenu.Value.MenuSwitch();
                    break;
            }
        }
    }
}
