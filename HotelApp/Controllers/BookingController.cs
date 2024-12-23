using HotelApp.Services.BookingService;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;

namespace HotelApp.Controllers
{
    /// <summary>
    /// Klassen hanterar undermenyn BookingMenu
    /// </summary>
    public class BookingController : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu>_serviceMenu;
        private readonly BookingService _bookingService;

        public BookingController(Lazy<ServiceMenu> serviceMenu, DisplayList displayList, BookingService bookingService)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
            _bookingService = bookingService;
        }
        public void MenuSwitch()
        {
            List<string> listBookingMenu = new List<string>
            {
                "Sök & visa bokning", "Skapa ny bokning", "Visa 100 tidigare bokningar", "Visa 100 kommande bokningar", "Ändra en bokning", "Avboka en bokning"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(listBookingMenu, false, Graphics.GetHeaderAsString("Meny Bokningar ↑/↓/↩"), true))
                {
                    case 0:
                        _bookingService.SearchBookingToList(false, false);
                        break;
                    case 1:
                        _bookingService.CheckAvailability();
                        break;
                    case 2:
                        _bookingService.Get100ByStartDate(true);
                        break;
                    case 3:
                        _bookingService.Get100ByStartDate(false);
                        break;
                    case 4:
                        _bookingService.SearchBookingToList(false, true);
                        break;
                    case 5:
                        _bookingService.SearchBookingToList(true, false);
                        break;
                    case 6:
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
}
