using Autofac;
using HotelApp.Controllers;
using HotelApp.Data;
using HotelApp.DI;
using HotelApp.Services;
using HotelApp.Services.BookingService;
using HotelApp.Services.BookingServices;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.Services.RoomServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            App app = new App();
            app.Run();
        }
    }
}
