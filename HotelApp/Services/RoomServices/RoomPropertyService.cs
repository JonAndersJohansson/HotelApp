using HotelApp.UI.Menus;
using HotelApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelApp.Models;
using HotelApp.Services;
using HotelApp.Utilities;

namespace HotelApp.Services.RoomServices
{
    public class RoomPropertyService : IPropertyService
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<RoomMenu> _roomMenu;
        private readonly RoomService _roomService;
        public RoomPropertyService(Lazy<RoomMenu> roomMenu, DisplayList displayList, RoomService roomService)
        {
            _displayList = displayList;
            _roomMenu = roomMenu;
            _roomService = roomService;
        }

        public readonly List<string> listRoomController = new List<string>
        {
        "Rumsnummer", "Typ av rum", "Antal möjliga extrasängar", "Baskostnad per natt", "Handikappanpassad", "Övrigt / Beskrivning", "Aktiv / Icke aktiv", "Kontrollera & Spara"
        };


        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayModel.
        /// </summary>
        public void PropertySwitch(Room room, bool isNew)
        {
            while (true)
            {
                switch (_displayList.BrowseAList(listRoomController, false, Graphics.GetHeaderAsString("Välj en av rummets egenskaper ↑/↓/↩"), false))
                {
                    case 0:
                        room.RoomNumberAsID = _roomService.GetRoomNumber();
                        break;
                    case 1:
                        room.RoomType = _roomService.GetRoomType();
                        break;
                    case 2:
                        room.NumberOfPossibleExtraBeds = _roomService.GetNumberOfPossibleBeds();
                        break;
                    case 3:
                        room.CostPerNight = _roomService.GetCostPerNight();
                        break;
                    case 4:
                        room.IsDisabilityFriendly = _roomService.GetYesOrNo(false);
                        break;
                    case 5:
                        room.OtherOrDescription = _roomService.GetOtherOrDescription();
                        break;
                    case 6:
                        room.IsActive = _roomService.GetYesOrNo(true);
                        break;
                    case 7:
                        if (_roomService.ValidateRoom(room, isNew) == true)
                        {
                            _roomService.AddRoom(room);
                            _roomMenu.Value.MenuSwitch();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n  Ogiltigt värde, var vänlig fyll i alla obligatoriska egenskaper.\n  Tryck valfri tangent för att försöka igen...");
                            Console.ReadKey();
                            break;
                        }
                    case 8:
                        _roomMenu.Value.MenuSwitch();
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ 'RoomController', tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        _roomMenu.Value.MenuSwitch();
                        break;
                }
            }
        }
    }
}
