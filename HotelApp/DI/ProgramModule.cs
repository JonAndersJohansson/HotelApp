﻿using Autofac;
using HotelApp.Data;
using HotelApp.Services;
using HotelApp.Services.BookingService;
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
            builder.RegisterType<App>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<DisplayList>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<MainMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<ServiceMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<CustomerMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<BookingMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<RoomMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<InvoiceMenu>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<RoomPropertyService>()
                    .AsSelf()
                    .SingleInstance();
            builder.RegisterType<RoomService>()
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
