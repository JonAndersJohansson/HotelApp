using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Utilities
{
    public static class Messages
    {
        public static void RequiredInputMessage()
        {
            Console.Write("  Krav:");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(72, currentLineCursor);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("'Exit' = Återgå ");
            Console.ResetColor();
        }
        public static void SetValueWithCursor()
        {
            Console.WriteLine("\n  Ange värde: ");
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(14, currentLineCursor - 1);
        }
        public static void SuccessfullInput()
        {
            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine("\n  Värde tillagt. Glöm inte spara.");
            Console.ResetColor();
            Thread.Sleep(1000);
        }

    }
}
