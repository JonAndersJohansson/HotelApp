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
    /// Klassen hanterar undermenyn BookingMenu
    /// </summary>
    public class BookingMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;

        public BookingMenu(Lazy<ServiceMenu> serviceMenu, DisplayList displayList)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
        }

        public readonly List<string> listBookingMenu = new List<string>
        {
        "Visa alla bokningar", "Skapa en ny bokning", "Ändra en befintlig bokning", "Avboka en registrerad bokning"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
            switch (_displayList.BrowseAList(listBookingMenu, false, Graphics.MakeHeader("bokningar ↑/↓"), true))
            {
                case 0:
                    //Visa alla bokningar
                    break;
                case 1:
                    //Skapa en ny bokning
                    break;
                case 2:
                    //Uppdatera en befintlig bokning
                    break;
                case 3:
                    //Avboka en bokning
                    break;
                case 4:
                    _serviceMenu.Value.MenuSwitch();
                    return;
                default:
                    Console.WriteLine("Ogiltigt alternativ 'BookingMenu', tryck valfri tangent för att återgå.");
                    Console.ReadKey();
                    _serviceMenu.Value.MenuSwitch();
                    break;
            }
        }
    }
}
