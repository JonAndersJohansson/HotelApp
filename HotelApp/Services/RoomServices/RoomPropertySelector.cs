using HotelApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelApp.Services;
using HotelApp.Utilities;
using HotelApp.Data.Models;
using HotelApp.Controllers;
using HotelApp.UI.Menus;

namespace HotelApp.Services.RoomServices
{
    public class RoomPropertySelector
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<RoomController> _roomController;
        private readonly RoomService _roomService;
        public RoomPropertySelector(Lazy<RoomController> roomController, DisplayList displayList, RoomService roomService)
        {
            _displayList = displayList;
            _roomController = roomController;
            _roomService = roomService;
        }
        public void PropertySwitch(Room room, bool isNew)
        {
            List<string> menuListInRoomProperty = new List<string>
            {
                "Rumsnummer *", "Typ av rum *", "Antal möjliga extrasängar", "Baskostnad per natt *", "Handikappanpassad", "Övrigt / Beskrivning", "Aktiv / Icke aktiv", "Kontrollera & Spara"
            };

            while (true)
            {
                string messageToUseInHeader = "Skapa nytt rum. Välj bland rummets egenskaper (* = Krav) ↑/↓/↩";
                if (!isNew)
                    messageToUseInHeader = $"Ändra rum. Välj bland rum {room.RoomNumber}´s egenskaper (* = Krav) ↑/↓/↩";

                switch (_displayList.BrowseAList(menuListInRoomProperty, false, Graphics.GetHeaderAsString(messageToUseInHeader), true))
                {
                    case 0:
                        _roomService.GetRoomNumber(room, isNew);
                        break;
                    case 1:
                        _roomService.GetRoomType(room, isNew);
                        break;
                    case 2:
                        _roomService.GetNumberOfPossibleBeds(room, isNew);
                        break;
                    case 3:
                        _roomService.GetCostPerNight(room, isNew);
                        break;
                    case 4:
                        _roomService.GetIsDisabilityFriendly(room, isNew);
                        break;
                    case 5:
                        _roomService.GetOtherOrDescription(room, isNew);
                        break;
                    case 6:
                        _roomService.GetIsActive(room, isNew);
                        break;
                    case 7:
                        if (_roomService.ValidateRoom(room, isNew) == true)
                        {
                            _roomService.AddRoom(room);
                            _roomController.Value.MenuSwitch();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n  Ogiltigt värde, var vänlig fyll i alla obligatoriska egenskaper.\n  Tryck valfri tangent för att försöka igen...");
                            Console.ReadKey();
                            break;
                        }
                    case 8:
                        _roomController.Value.MenuSwitch();
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ i RoomPropertyService switch, tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        _roomController.Value.MenuSwitch();
                        break;
                }
            }
        }
    }
}
