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
            Console.WriteLine("\n  Värde tillagt.");
            Console.ResetColor();
            Thread.Sleep(1000);
        }

        public static void ClearAndShowHeader(string headerText)
        {
            Console.Clear();
            Graphics.ShowMainGraphics();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Graphics.GetHeaderAsString(headerText));
            Console.ResetColor();
        }

        public static void AbortBooking()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n  Avbryter bokning...");
            Console.ResetColor();
            Thread.Sleep(1000);
        }
    }
}
