using HotelApp.UI.Menus;
using HotelApp.UX;
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
        private readonly Lazy<MainMenu> _mainMenu;
        public DisplayList(Lazy<MainMenu> mainMenu)
        {
            _mainMenu = mainMenu;
        }
        public int BrowseAList<T>(List<T> genericList, bool isMainMenu, string? header, bool isMenu)
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
                    Console.WriteLine("Välj alternativ med piltangenterna upp/ned och tryck enter.");
                Console.ResetColor();

                if (genericList.Count == 0)
                {
                    Console.WriteLine("Inga värden kunde hittas, försök igen.");
                    break;
                }
                    

                DisplayItems(genericList, selectedIndex);

                if (!isMainMenu && isMenu)
                    ShowBackButton(selectedIndex == genericList.Count);
                else if (!isMainMenu && !isMenu)
                    ShowAbortButton(selectedIndex == genericList.Count);

                while (userInputIsUnsatisfying)
                {
                    newIndex = HandleUserInput(genericList, selectedIndex,
                    isMainMenu, isMenu);

                    if (newIndex != -1)
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

        private void DisplayItems<T>(List<T> lista, int selectedIndex)
        {
            const int padding = 2; // Antal tomma utrymmen från vänster

            for (int i = 0; i < lista.Count; i++)
            {
                Console.Write(new string(' ', padding)); // Lägg till marginal

                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(">> ");
                    Console.ResetColor();
                }
                else if (lista[i].ToString() == "Avsluta")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ResetColor();
                }
                if (lista[i].ToString() == "Avsluta")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{lista[i].ToString()}\n");
                    Console.ResetColor();
                }
                else
                    Console.Write($"{lista[i].ToString()} \n");
                Console.ResetColor();
            }
        }

        private void ShowBackButton(bool isSelected)
        {
            const int padding = 2; // Antal tomma utrymmen från vänster
            Console.Write(new string(' ', padding)); // Lägg till marginal

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
            const int padding = 2; // Antal tomma utrymmen från vänster
            Console.Write(new string(' ', padding)); // Lägg till marginal

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

            Console.WriteLine("Avbryt och återgå till huvudmenyn");
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
                    _mainMenu.Value.MenuSwitch();
                else
                    return selectedIndex;
            }
            else
                return -1;

            return selectedIndex;
        }
    }
}
