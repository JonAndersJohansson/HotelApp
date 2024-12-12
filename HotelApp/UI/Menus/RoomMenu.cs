using HotelApp.Data;
using HotelApp.Models;
using HotelApp.Services;
using HotelApp.Services.RoomServices;
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
    /// Klassen hanterar undermenyn RoomMenu
    /// </summary>
    public class RoomMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;
        private readonly Lazy<RoomPropertyService> _roomPropertyService;
        private readonly RoomService _roomService; 

        public RoomMenu(Lazy<ServiceMenu> serviceMenu, DisplayList displayList, RoomService roomService, Lazy<RoomPropertyService> roomPropertyService)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
            _roomService = roomService;
            _roomPropertyService = roomPropertyService;
        }

        public readonly List<string> listRoomMenu = new List<string>
        {
        "Visa alla rum (Aktiva och icke aktiva)", "Lägg till ett nytt rum", "Ändra ett befintligt rum", "Avaktivera/aktivera ett rum"
        };

        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
            switch (_displayList.BrowseAList(listRoomMenu, false, Graphics.GetHeaderAsString("Meny Rum ↑/↓/↩"), true))
            {
                case 0:
                    _roomService.ReadOneRoom(_roomService.GetARoom(_roomService.GetARoomIndex()));
                    break;
                case 1:
                    var newRoom = new Room { RoomNumberAsID = -1, CostPerNight = 0, RoomType = BedSize.Single };
                    _roomPropertyService.Value.PropertySwitch(newRoom, true);
                    break;
                case 2:
                    _roomPropertyService.Value.PropertySwitch(_roomService.GetARoom(_roomService.GetARoomIndex()), false);
                    break;
                case 3:
                    _roomService.DeactivateARoom((_roomService.GetARoom(_roomService.GetARoomIndex())));
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
