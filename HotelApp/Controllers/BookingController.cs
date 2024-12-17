using HotelApp.Services.BookingService;
using HotelApp.Services.CustomerServices;
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
                "Sök bokning", "Skapa ny bokning", "Visa 100 tidigare bokningar", "Visa 100 kommande bokningar", "Lägg till Övrig info på bokning", "Avboka en bokning"
            };
            switch (_displayList.BrowseAList(listBookingMenu, false, Graphics.GetHeaderAsString("Meny Bokningar ↑/↓/↩"), true))
            {
                case 0:
                    _bookingService.ReadOneBooking(_bookingService.GetABookingInList(_bookingService.GetListOfBookingsBySearch(false, false), false, false), false);
                    break;
                case 1:
                    //Skapa en ny bokning
                    break;
                case 2:
                    //Visa 100 tidigare bokningar
                    break;
                case 3:
                    //Visa 100 kommande bokningar
                    break;
                case 4:
                    //Lägg till övrig info
                    break;
                case 5:
                    //Avboka en bokning
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
