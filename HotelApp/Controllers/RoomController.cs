using HotelApp.Data;
using HotelApp.Data.Models;
using HotelApp.Services;
using HotelApp.Services.RoomServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Controllers
{
    /// <summary>
    /// Klassen hanterar undermenyn CustomerMenu
    /// </summary>
    public class RoomController : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;
        private readonly Lazy<RoomPropertySelector> _roomPropertySelector;
        private readonly RoomService _roomService;

        public RoomController(Lazy<ServiceMenu> serviceMenu, DisplayList displayList, RoomService roomService, Lazy<RoomPropertySelector> roomPropertySelector)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
            _roomService = roomService;
            _roomPropertySelector = roomPropertySelector;
        }
        public void MenuSwitch()
        {
            List<string> listRoomMenu = new List<string>
            {
                "Visa alla rum (och välj ett)", "Lägg till ett nytt rum", "Ändra ett befintligt rum", "Avaktivera/aktivera ett rum"
            };
            switch (_displayList.BrowseAList(listRoomMenu, false, Graphics.GetHeaderAsString("Meny Rum ↑/↓/↩"), true))
            {
                case 0:
                    _roomService.ReadOneRoom(_roomService.GetARoom(_roomService.GetARoomIndex(false, false)));
                    break;
                case 1:
                    var newRoom = new Room { RoomNumber = -1, CostPerNight = -1, RoomType = BedSize.Single };
                    _roomPropertySelector.Value.PropertySwitch(newRoom, true);
                    break;
                case 2:
                    _roomPropertySelector.Value.PropertySwitch(_roomService.GetARoom(_roomService.GetARoomIndex(false, true)), false);
                    break;
                case 3:
                    _roomService.DeactivateARoom(_roomService.GetARoom(_roomService.GetARoomIndex(true, false)));
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
