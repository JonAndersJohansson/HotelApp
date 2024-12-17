using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.UI
{
    /// <summary>
    /// Klassen hanterar listor genom att låta användaren stega igenom dem 
    /// och välja ett index.
    /// </summary>
    public class DisplayList
    {
        //private readonly IMenu _mainMenu;
        //public DisplayList(IMenu mainMenu )
        //{
        //    _mainMenu = mainMenu;
        //}
        public int BrowseAList<T>(List<T> genericList, bool isMainMenuInSwitch, string? header, bool isMenuInSwitch)
        {
            int newIndex = 0;
            int selectedIndex = 0;
            bool userInputIsUnsatisfying = true;

            while (userInputIsUnsatisfying)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();

                Console.ForegroundColor = ConsoleColor.Blue;
                if (header != null)
                    Console.WriteLine(header);
                else
                    Console.WriteLine("Välj alternativ ↑/↓/↩");
                Console.ResetColor();

                if (genericList.Count == 0)
                {
                    Console.WriteLine("Inga värden kunde hittas, försök igen.");
                    break;
                }
                    

                DisplayItems(genericList, selectedIndex);

                if (!isMainMenuInSwitch && isMenuInSwitch)
                    ShowBackButton(selectedIndex == genericList.Count);
                else if (!isMainMenuInSwitch && !isMenuInSwitch)
                    ShowAbortButton(selectedIndex == genericList.Count);

                while (true)
                {
                    newIndex = HandleUserInput(genericList, selectedIndex,
                    isMainMenuInSwitch, isMenuInSwitch);

                    if (newIndex == -1)
                        return -1;
                    else if (newIndex != -2)
                        break;
                    else if (newIndex == -2)
                        continue;
                    else
                        break;
                }
                if (newIndex != selectedIndex)
                    selectedIndex = newIndex;
                else
                    userInputIsUnsatisfying = false;

            }
            if (genericList.Count == 0)  
                return -1;
            return selectedIndex;
        }

        private void DisplayItems<T>(List<T> genericList, int selectedIndex)
        {
            const int padding = 2; 

            for (int i = 0; i < genericList.Count; i++)
            {
                Console.Write(new string(' ', padding)); 

                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(">> ");
                    Console.ResetColor();
                }
                else if (genericList[i].ToString() == "Avsluta")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (genericList[i].ToString() == "Kontrollera & Spara")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ResetColor();
                }
                if (genericList[i].ToString() == "Avsluta")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{genericList[i].ToString()}\n");
                    Console.ResetColor();
                }
                else if (genericList[i].ToString() == "Kontrollera & Spara")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{genericList[i].ToString()}\n");
                    Console.ResetColor();
                }
                else
                    Console.Write($"{genericList[i].ToString()} \n");
                Console.ResetColor();
            }
        }

        private void ShowBackButton(bool isSelected)
        {
            const int padding = 2;
            Console.Write(new string(' ', padding));

            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(">> ");
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine("Tillbaka");
            Console.ResetColor();
        }
        private void ShowAbortButton(bool isSelected)
        {
            const int padding = 2;
            Console.Write(new string(' ', padding));

            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(">> ");
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine("Avbryt");
            Console.ResetColor();
        }

        private int HandleUserInput<T>(List<T> lista, int selectedIndex,
            bool isMainMenu, bool isMenu)
        {
            var keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedIndex--;

                if (selectedIndex < 0)
                    selectedIndex = isMainMenu ? lista.Count - 1 : lista.Count;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedIndex++;

                if (selectedIndex > (isMainMenu ? lista.Count - 1 : lista.Count))
                    selectedIndex = 0;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (selectedIndex == lista.Count && !isMenu)
                {
                    return -1;
                }
                else
                    return selectedIndex;
            }
            else
                return -2;

            return selectedIndex;
        }
    }
}
