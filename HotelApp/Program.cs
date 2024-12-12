using Autofac;
using HotelApp.Data;
using HotelApp.DI;
using HotelApp.Services;
using HotelApp.Services.BookingService;
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
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var myContainerBuilder = new ContainerBuilder();
            myContainerBuilder.RegisterModule<ProgramModule>();
            var myContainer = myContainerBuilder.Build();

            var app = myContainer.Resolve<App>();
            var displayList = myContainer.Resolve<DisplayList>();
            var mainMenu = myContainer.Resolve<MainMenu>();
            var serviceMenu = myContainer.Resolve<ServiceMenu>();
            var bookingMenu = myContainer.Resolve<BookingMenu>();
            var customerMenu = myContainer.Resolve<CustomerMenu>();
            var roomMenu = myContainer.Resolve<RoomMenu>();
            var invoiceMenu = myContainer.Resolve<InvoiceMenu>();

            var readRoom = myContainer.Resolve<RoomService>();
            var roomController = myContainer.Resolve<RoomPropertyService>();

            var dataInitializer = myContainer.Resolve<DataInitializer>();
            var dbContext = myContainer.Resolve<ApplicationDbContext_FAKE>();






            dataInitializer.MigrateAndSeedData(dbContext);

            //var inputHandler = myContainer.Resolve<IInputHandler>();
            //var create = myContainer.Resolve<Create>();
            //var read = myContainer.Resolve<Read>();
            //var update = myContainer.Resolve<Update>();
            //var delete = myContainer.Resolve<Delete>();

            app.Run();
        }
    }
}
