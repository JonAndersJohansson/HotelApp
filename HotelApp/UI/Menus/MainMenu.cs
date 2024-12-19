using HotelApp.Services.BookingService;
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
    /// Klassen hanterar huvudmenyn MainMenu
    /// </summary>
    public class MainMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;
        private readonly BookingService _bookingService;
        private readonly SearchMenu _searchMenu;
        public MainMenu(DisplayList displayList, Lazy<ServiceMenu> serviceMenu, BookingService bookingService, SearchMenu searchMenu)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
            _bookingService = bookingService;
            _searchMenu = searchMenu;
        }
        public void MenuSwitch()
        {
            List<string> listMainMenu = new List<string>
            {
                "Sök & Visa", "Ny Bokning", "Visa nuvarande gäster", "Hantera - Kunder/Bokningar/Rum/Fakturor", "Avsluta"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(listMainMenu, true, Graphics.GetHeaderAsString("Huvudmeny ↑/↓/↩"), true))
                {
                    case 0:
                        _searchMenu.MenuSwitch();
                        break;
                    case 1:
                        _bookingService.CheckAvailability();
                        break;
                    case 2:
                        _bookingService.SearchCurrentVisitors();
                        break;
                    case 3:
                        _serviceMenu.Value.MenuSwitch();
                        break;
                    case 4:
                        Environment.Exit(0);
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ 'MainMenu', tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        break;
                }
            }
            
        }
    }
}
