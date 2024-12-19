using Autofac;
using HotelApp.Controllers;
using HotelApp.Data;
using HotelApp.Services;
using HotelApp.Services.BookingService;
using HotelApp.Services.BookingServices;
using HotelApp.Services.CustomerServices;
using HotelApp.Services.InvoiceServices;
using HotelApp.Services.RoomServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.DI
{
    public class ProgramModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DisplayList>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<MainMenu>()
                    .As<IMenu>()
                    .SingleInstance();
            builder.RegisterType<ServiceMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<SearchMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<BookingController>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<CustomerController>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<InvoiceController>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<RoomController>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<RoomPropertySelector>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<RoomService>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<CustomerPropertySelector>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<CustomerService>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<InvoicePropertySelector>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<InvoiceService>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<BookingService>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<DataInitializer>()
                    .AsSelf();
            builder.RegisterType<ApplicationDbContext_FAKE>()
                    .SingleInstance();

            //builder.RegisterType<InputHandler>()
            //       .As<IInputHandler>();
            //builder.RegisterType<Create>()
            //        .AsSelf()
            //        .SingleInstance();
            //builder.RegisterType<Read>()
            //        .AsSelf()
            //        .SingleInstance();
            //builder.RegisterType<Update>()
            //        .AsSelf()
            //        .SingleInstance();
            //builder.RegisterType<Delete>()
            //        .AsSelf()
            //        .SingleInstance();
        }
    }
}
