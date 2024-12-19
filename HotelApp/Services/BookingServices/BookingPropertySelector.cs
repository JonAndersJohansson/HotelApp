using HotelApp.Data.Models;
using HotelApp.Services.BookingService;
using HotelApp.UI;
using HotelApp.Utilities;

namespace HotelApp.Services.BookingServices
{
    //public class BookingPropertySelector
    //{
    //    private readonly DisplayList _displayList;
    //    private readonly BookingService _bookingService;
    //    public BookingPropertySelector(DisplayList displayList, BookingService bookingService)
    //    {
    //        _displayList = displayList;
    //        _bookingService = bookingService;
    //    }
    //    public void PropertySwitch(Booking booking)
    //    {
    //        List<string> menuListInCustomerPropertySelector = new List<string>
    //        {
    //            "Lägg till/ändra övrig information", "Annulera bokning", "Kontrollera & Spara"
    //        };
    //        while (true)
    //        {
    //            switch (_displayList.BrowseAList(menuListInCustomerPropertySelector, false, Graphics.GetHeaderAsString("Ändra i bokning"), true))
    //            {
    //                case 0:
    //                    _bookingService.GetOtherInfo();
    //                    break;
    //                case 1:
    //                    _bookingService.CancelBooking();
    //                    break;
    //                case 2:
    //                    if (_bookingService.ValidateBooking() == true)
    //                        _bookingService.SaveBookingToDataBase(booking);
    //                    else
    //                    {
    //                        Console.WriteLine("\n  Ogiltigt värde, var vänlig fyll i alla obligatoriska uppgifter.\n  Tryck valfri tangent för att försöka igen...");
    //                        Console.ReadKey();
    //                        break;
    //                    }
    //                    break;
    //                case 9:
    //                    return;
    //                default:
    //                    Console.WriteLine("Ogiltigt alternativ i CustomerPropertySelector switch, tryck valfri tangent för att återgå.");
    //                    Console.ReadKey();
    //                    return;
    //            }
    //        }
    //    }
    //}
}
