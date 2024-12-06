using HotelApp.UI.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Core
{
    public class App
    {
        private readonly MainMenu _mainMenu;
        public App(MainMenu mainMenu)
        {
            _mainMenu = mainMenu;
        }
        public void Run()
        {
            //Seed
            //Update ALL bools in models
            _mainMenu.MenuSwitch();
        }
    }
}
