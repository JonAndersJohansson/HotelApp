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

            var myContainerBuilder = new ContainerBuilder();
            myContainerBuilder.RegisterModule<ProgramModule>();
            var myContainer = myContainerBuilder.Build();

            var dataInitializer = myContainer.Resolve<DataInitializer>();

            var displayList = myContainer.Resolve<DisplayList>();
            var mainMenu = myContainer.Resolve<IMenu>();
            var serviceMenu = myContainer.Resolve<ServiceMenu>();
            var searchMenu = myContainer.Resolve<SearchMenu>();
            var bookingController = myContainer.Resolve<BookingController>();
            var customerController = myContainer.Resolve<CustomerController>();
            var invoiceController = myContainer.Resolve<InvoiceController>();
            var roomController = myContainer.Resolve<RoomController>();
            var dbContext1 = myContainer.Resolve<ApplicationDbContext>();
            var roomService = myContainer.Resolve<RoomService>();
            var roomPropertyService = myContainer.Resolve<RoomPropertySelector>();
            var customerService = myContainer.Resolve<CustomerService>();
            var customerPropertyService = myContainer.Resolve<CustomerPropertySelector>();
            var bookingPropertyService = myContainer.Resolve<BookingPropertySelector>();
            var invoiceService = myContainer.Resolve<InvoiceService>();
            var bookingService = myContainer.Resolve<BookingService>();

            invoiceService.CheckOverDue();
            mainMenu.MenuSwitch();
        }
    }
}
