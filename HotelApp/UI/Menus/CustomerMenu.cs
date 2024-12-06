using HotelApp.UI;
using HotelApp.UX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.UI.Menus
{
    /// <summary>
    /// Klassen hanterar undermenyn CustomerMenu
    /// </summary>
    public class CustomerMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;

        public CustomerMenu(Lazy<ServiceMenu> serviceMenu, DisplayList displayList)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
        }

        public readonly List<string> listCustomerMenu = new List<string>
        {
        "Visa alla kunder", "Lägg till en ny kund", "Ändra kundinformation på befintlig kund", "Ta bort en kund (om inga bokningar finns kopplade)"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
            switch (_displayList.BrowseAList(listCustomerMenu, false, Graphics.MakeHeader("kunder ↑/↓"), true))
            {
                case 0:
                    //Visa alla kunder
                    break;
                case 1:
                    //Lägg till en ny kund
                    break;
                case 2:
                    //Uppdatera kundinformation på befintlig kund
                    break;
                case 3:
                    //Ta bort en kund (om inga bokningar finns kopplade)
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
