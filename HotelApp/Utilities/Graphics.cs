using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Utilities
{
    public static class Graphics
    {
        public static void ShowMainGraphics()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════════════╗\n" +
                "║ _    ____ _  _ ____    _  _ ____ ____ ___  _ ____ ___    ____ ____ ____ ____ ____ ___ ║\n" +
                "║ |    |__| |_/  |___    |  | |___ |__/ |  \\ | |     |     |__/ |___ [__  |  | |__/  |  ║\n" +
                "║ |___ |  | | \\_ |___     \\/  |___ |  \\ |__/ | |___  |     |  \\ |___ ___] |__| |  \\  |  ║\n" +
                "╠═══════════════════════════════════════════════════════════════════════════════════════╣");
            Console.ResetColor();
        }
        public static string GetHeaderAsString(string textToDisplay)
        {
            int totalLength = 87;
            int padding = totalLength - textToDisplay.Length;
            int leftPadding = padding / 2;
            int rightPadding = padding - leftPadding;
            string centeredText = new string(' ', leftPadding) + textToDisplay + new string(' ', rightPadding);

            return $"║{centeredText}║\n╚═══════════════════════════════════════════════════════════════════════════════════════╝";
        }
    }
}
