using HotelApp.Data.Models;
using HotelApp.Services;
using HotelApp.Services.BookingService;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.UI.Menus
{
    public class SearchMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly BookingService _bookingService;
        private readonly CustomerService _customerService;
        private readonly InvoiceService _invoiceService;
        private readonly RoomService _roomService;
        public SearchMenu(DisplayList displayList, BookingService bookingService, CustomerService customerService, InvoiceService invoiceService, RoomService roomService)
        {
            _displayList = displayList;
            _bookingService = bookingService;
            _customerService = customerService;
            _invoiceService = invoiceService;
            _roomService = roomService; 
        }
        public void MenuSwitch()
        {
            List<string> listSearchMenu = new List<string>
            {
                "Sök Bokning", "Sök Kund", "Sök Faktura", "Visa alla rum"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(listSearchMenu, false, Graphics.GetHeaderAsString("Sökmeny ↑/↓/↩"), true))
                {
                    case 0:
                        _bookingService.SearchBookingToList(false, false);
                        break;
                    case 1:
                        _customerService.SearchCustomerToList(false, false);
                        break;
                    case 2:
                        _invoiceService.SearchInvoiceToList(false, false);
                        break;
                    case 3:
                        _roomService.SelectRoomIndex(false, false);
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ 'SearchMenu', tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        break;
                }
            }

        }
    }
}
