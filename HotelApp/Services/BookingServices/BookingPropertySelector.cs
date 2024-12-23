using HotelApp.Controllers;
using HotelApp.Data.Models;
using HotelApp.Services.BookingService;
using HotelApp.UI;
using HotelApp.Utilities;

namespace HotelApp.Services.BookingService
{
    public class BookingPropertySelector
    {
        private readonly DisplayList _displayList;
        private readonly BookingService _bookingService;
        public BookingPropertySelector(DisplayList displayList, BookingService bookingService)
        {
            _displayList = displayList;
            _bookingService = bookingService;
        }
        public void PropertySwitch(Booking booking)
        {
            List<string> menuListInBookingPropertySelector = new List<string>
            {
                "Ändra Incheckningsdatum", "Ändra Utcheckningsdatom", "Ändra antal gäster", "Ändra bokade rum", "Ändra övrig info", "Kontrollera & Spara"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(menuListInBookingPropertySelector, false, Graphics.GetHeaderAsString($"Ändra bokningsNr: {booking.Id}"), true))
                {
                    case 0:
                        _bookingService.ChangeDateByCalendar(booking, "Ända Incheckningsdatum", true);
                        break;
                    case 1:
                        _bookingService.ChangeDateByCalendar(booking, "Ända Uncheckningsdatum", false);
                        break;
                    case 2:
                        _bookingService.ChangeNumberOfGuests(booking);
                        break;
                    case 3:
                        _bookingService.ChangeRoomsInBooking(booking);
                        break;
                    case 4:
                        _bookingService.ChangeOtherInfo(booking);
                        break;
                    case 5:
                        if (_bookingService.ValidateBooking(booking) == true)
                        {
                            _bookingService.SaveChangesOnBookingToDataBase(booking);
                            return;
                        }
                        else
                            break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ i BookingPropertySelector switch, tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
