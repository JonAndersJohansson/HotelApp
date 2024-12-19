using HotelApp.Data.Models;
using HotelApp.Services;
using HotelApp.Services.RoomServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;

namespace HotelApp.Controllers
{
    /// <summary>
    /// Klassen hanterar undermenyn CustomerMenu
    /// </summary>
    public class RoomController : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<RoomPropertySelector> _roomPropertySelector;
        private readonly RoomService _roomService;

        public RoomController(DisplayList displayList, RoomService roomService, Lazy<RoomPropertySelector> roomPropertySelector)
        {
            _displayList = displayList;
            _roomService = roomService;
            _roomPropertySelector = roomPropertySelector;
        }
        public void MenuSwitch()
        {
            List<string> listRoomMenu = new List<string>
            {
                "Visa alla rum (och välj ett)", "Lägg till ett nytt rum", "Ändra ett befintligt rum", "Avaktivera/aktivera ett rum"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(listRoomMenu, false, Graphics.GetHeaderAsString("Meny Rum ↑/↓/↩"), true))
                {
                    case 0:
                        _roomService.SelectRoomIndex(false, false);
                        break;
                    case 1:
                        var newRoom = new Room { RoomNumber = -1, CostPerNight = -1, RoomType = BedSize.Single };
                        _roomPropertySelector.Value.PropertySwitch(newRoom, true);
                        break;
                    case 2:
                        _roomService.SelectRoomIndex(false, true);
                        break;
                    case 3:
                        _roomService.SelectRoomIndex(true, false);
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ 'CustomerMenu', tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        return;
                }
            }
            
        }
    }
}
