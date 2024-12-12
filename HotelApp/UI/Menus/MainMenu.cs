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
        private readonly string _mainMenuHeader = "Hej";
        public MainMenu(DisplayList displayList, Lazy<ServiceMenu> serviceMenu)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
        }

        public readonly List<string> listMainMenu = new List<string>
        {
        "Sök", "Ny Bokning", "Visa besökande gäster", "Hantera - Kunder/Bokningar/Rum/Fakturor", "Avsluta"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i huvudmenyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
            switch (_displayList.BrowseAList(listMainMenu, true, Graphics.GetHeaderAsString("Huvudmeny ↑/↓/↩"), true))
            {
                case 0:
                    //Sök 
                    break;
                case 1:
                    // Ny Bokning
                    break;
                case 2:
                    // Besökande
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
                    MenuSwitch();
                    break;
            }
        }
    }
}
