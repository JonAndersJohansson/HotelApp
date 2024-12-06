using HotelApp.UI;
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
        "1. Sök efter lediga rum och skapa bokning", "2. Sök efter bokning med bokningsnummer", "3. Sök efter kund med kundnummer", "4. Hantera kunder/bokningar/rum/fakturor", "Avsluta"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i huvudmenyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
            switch (_displayList.BrowseAList(listMainMenu, true, "╔═════════════════════════════════════════╗\n║                HUVUDMENY                ║\n╚═════════════════════════════════════════╝", true))
            {
                case 0:
                    //Sök efter lediga rum och skapa bokning
                    break;
                case 1:
                    //Sök efter bokning med bokningsnummer
                    break;
                case 2:
                    //Sök efter kund med kundnummer
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
