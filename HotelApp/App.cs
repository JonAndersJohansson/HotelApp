using Autofac;
using HotelApp.Controllers;
using HotelApp.Data;
using HotelApp.DI;
using HotelApp.Services;
using HotelApp.Services.BookingService;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.Services.RoomServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelApp
{
    public class App
    {
        public void Run()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);

            using (var dbContext = new ApplicationDbContext(options.Options))
            {
                var dataInitiaizer = new DataInitializer(dbContext);
                dataInitiaizer.MigrateAndSeedData();
            }

            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Console.SetWindowSize(90, 50);

            var myContainerBuilder = new ContainerBuilder();
            myContainerBuilder.RegisterModule<ProgramModule>();
            var myContainer = myContainerBuilder.Build();


            var displayList = myContainer.Resolve<DisplayList>();
            var mainMenu = myContainer.Resolve<IMenu>();
            var serviceMenu = myContainer.Resolve<ServiceMenu>();
            var searchMenu = myContainer.Resolve<SearchMenu>();
            var bookingMenu = myContainer.Resolve<BookingController>();
            var customerMenu = myContainer.Resolve<CustomerController>();
            var invoiceMenu = myContainer.Resolve<InvoiceController>();
            var roomMenu = myContainer.Resolve<RoomController>();

            var dbContext1 = myContainer.Resolve<ApplicationDbContext>();

            var roomService = myContainer.Resolve<RoomService>();
            var roomPropertyService = myContainer.Resolve<RoomPropertySelector>();

            var customerService = myContainer.Resolve<CustomerService>();
            var customerPropertyService = myContainer.Resolve<CustomerPropertySelector>();

            var invoiceService = myContainer.Resolve<InvoiceService>();
            var invoicePropertyService = myContainer.Resolve<InvoicePropertySelector>();

            var bookingService = myContainer.Resolve<BookingService>();

            var dataInitializer = myContainer.Resolve<DataInitializer>();


            





            //var inputHandler = myContainer.Resolve<IInputHandler>();
            //var create = myContainer.Resolve<Create>();
            //var read = myContainer.Resolve<Read>();
            //var update = myContainer.Resolve<Update>();
            //var delete = myContainer.Resolve<Delete>();


            mainMenu.MenuSwitch();
        }
    }
}
