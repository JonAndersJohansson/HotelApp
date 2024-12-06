using Autofac;
using HotelApp.Core;
using HotelApp.DI;
using HotelApp.UI;
using HotelApp.UI.Menus;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelApp.EntryPoint
{
    public class Program
    {
        static void Main(string[] args)
        {
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
            //var inputHandler = myContainer.Resolve<IInputHandler>();
            //var create = myContainer.Resolve<Create>();
            //var read = myContainer.Resolve<Read>();
            //var update = myContainer.Resolve<Update>();
            //var delete = myContainer.Resolve<Delete>();

            app.Run();
        }
    }
}
