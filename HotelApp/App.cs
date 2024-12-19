using Autofac;
using HotelApp.Controllers;
using HotelApp.Data;
using HotelApp.DI;
using HotelApp.Services.BookingService;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.Services.RoomServices;
using HotelApp.Services;
using HotelApp.UI;
using HotelApp.UI.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{
    public class App
    {
        public void Run()
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Console.SetWindowSize(90, 50);

            var myContainerBuilder = new ContainerBuilder();
            myContainerBuilder.RegisterModule<ProgramModule>();
            var myContainer = myContainerBuilder.Build();


            var displayList = myContainer.Resolve<DisplayList>();
            var mainMenu = myContainer.Resolve<IMenu>();
            var serviceMenu = myContainer.Resolve<ServiceMenu>();
            var bookingMenu = myContainer.Resolve<BookingController>();
            var customerMenu = myContainer.Resolve<CustomerController>();
            var invoiceMenu = myContainer.Resolve<InvoiceController>();
            var roomMenu = myContainer.Resolve<RoomController>();

            var roomService = myContainer.Resolve<RoomService>();
            var roomPropertyService = myContainer.Resolve<RoomPropertySelector>();

            var customerService = myContainer.Resolve<CustomerService>();
            var customerPropertyService = myContainer.Resolve<CustomerPropertySelector>();

            var invoiceService = myContainer.Resolve<InvoiceService>();
            var invoicePropertyService = myContainer.Resolve<InvoicePropertySelector>();

            var bookingService = myContainer.Resolve<BookingService>();

            var dataInitializer = myContainer.Resolve<DataInitializer>();
            var dbContext = myContainer.Resolve<ApplicationDbContext_FAKE>();



            dataInitializer.MigrateAndSeedData(dbContext);

            //var inputHandler = myContainer.Resolve<IInputHandler>();
            //var create = myContainer.Resolve<Create>();
            //var read = myContainer.Resolve<Read>();
            //var update = myContainer.Resolve<Update>();
            //var delete = myContainer.Resolve<Delete>();


            mainMenu.MenuSwitch();
        }
    }
}
