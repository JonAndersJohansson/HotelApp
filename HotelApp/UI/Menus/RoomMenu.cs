using HotelApp.Models;
using HotelApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.UI.Menus
{
    /// <summary>
    /// Klassen hanterar undermenyn RoomMenu
    /// </summary>
    public class RoomMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;

        public RoomMenu(Lazy<ServiceMenu> serviceMenu, DisplayList displayList)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
        }

        public readonly List<string> listRoomMenu = new List<string>
        {
        "1. Visa alla rum (Aktiva och icke aktiva)", "2. Lägg till ett nytt rum", "3. Uppdatera ett befintligt rum", "4. Avaktivera/aktivera ett rum"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {

            List<Room> exempelRum = new List<Room>
            {
                new Room { RoomNumber = 103, RoomType = BedSize.Single, CostPerNight = 1670 },
                new Room { RoomNumber = 204, RoomType = BedSize.Double, CostPerNight = 2400 },
                new Room { RoomNumber = 114, RoomType = BedSize.Double, CostPerNight = 2160 },
                new Room { RoomNumber = 122, RoomType = BedSize.Single, CostPerNight = 1670 },
                new Room { RoomNumber = 313, RoomType = BedSize.Single, CostPerNight = 1670 },
            };




            switch (_displayList.BrowseAList(listRoomMenu, false, "╔═════════════════════════════════════════╗\n║                   RUM                   ║\n╚═════════════════════════════════════════╝", true))
            {
                case 0:
                    var selectedIndex = _displayList.BrowseAList(exempelRum, false, "Välj ett rum att ta bort med piltangenterna upp/ned och tryck enter.\n", false);
                    
                    Room selectedRoom = exempelRum[selectedIndex];
                    exempelRum.RemoveAt(selectedIndex);
                    Console.WriteLine($"Följande rum är borttaget {selectedRoom}");
                    Console.ReadKey();
                    break;
                case 1:
                    //Lägg till ett nytt rum
                    break;
                case 2:
                    //Uppdatera ett befintligt rum
                    break;
                case 3:
                    //Avaktivera/aktivera ett rum
                    break;
                case 4:
                    _serviceMenu.Value.MenuSwitch();
                    return;
                default:
                    Console.WriteLine("Ogiltigt alternativ 'RoomMenu', tryck valfri tangent för att återgå.");
                    Console.ReadKey();
                    _serviceMenu.Value.MenuSwitch();
                    break;
            }
        }
    }
}
