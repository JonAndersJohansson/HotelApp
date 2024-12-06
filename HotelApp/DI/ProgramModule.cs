using Autofac;
using HotelApp.Core;
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
