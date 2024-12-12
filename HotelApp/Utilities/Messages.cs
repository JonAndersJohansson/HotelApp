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
            int currentLineCursor2 = Console.CursorTop;
            Console.SetCursorPosition(72, currentLineCursor2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("'Exit' = Återgå ");
            Console.ResetColor();
        }
        public static void SetValueWithCursor()
        {
            Console.WriteLine("\n  Ange värde: ");
            int currentLineCursor1 = Console.CursorTop;
            Console.SetCursorPosition(14, currentLineCursor1 - 1);
        }

    }
}
