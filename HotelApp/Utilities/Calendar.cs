using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Utilities
{
    public static class Calendar
    {
        public static DateTime GetDateTimeByCalendar(string headerCalendar) 
        {
            // Startdatum (början av månaden)
            DateTime currentDate = DateTime.Now;
            DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, 1);

            while (true)
            {
                Console.Clear();
                Graphics.ShowMainGraphics();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Graphics.GetHeaderAsString(headerCalendar));
                Console.ResetColor();
                RenderCalendar(selectedDate);

                // Läsa användarens tangent
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        selectedDate = selectedDate.AddDays(-1);
                        break;
                    case ConsoleKey.UpArrow:
                        selectedDate = selectedDate.AddDays(-7);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedDate = selectedDate.AddDays(7);
                        break;
                    case ConsoleKey.Enter:
                        AnsiConsole.MarkupLine($"\n  Du valde: [blue]{selectedDate:yyyy-MM-dd}[/]");
                        return selectedDate;
                    case ConsoleKey.Escape:
                        return DateTime.MinValue;
                }
            }
        }
        public static void RenderCalendar(DateTime selectedDate)
        {
            var calendarContent = new StringWriter();

            // Kalenderhuvud
            calendarContent.WriteLine($"[blue]{selectedDate:MMMM}[/]".ToUpper());
            calendarContent.WriteLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
            calendarContent.WriteLine("─────────────────────────────────");

            DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1; // Justera för måndag som veckostart

            // Fyll med tomma platser innan första dagen i månaden
            for (int i = 0; i < startDay; i++)
            {
                calendarContent.Write("     ");
            }

            // Skriv ut dagarna
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (day == selectedDate.Day)
                {
                    // Siffran 2 sätter minimum bredd (även om 1 siffra)
                    calendarContent.Write($"[blue]{day,2}[/]   ");
                }
                else
                {
                    calendarContent.Write($"{day,2}   ");
                }

                // Gå till nästa rad efter söndag
                if ((startDay + day) % 7 == 0)
                {
                    calendarContent.WriteLine();
                }
            }

            // Skapa en panel med dubbla kanter
            var panel = new Panel(calendarContent.ToString())
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader(($"[blue]{selectedDate:yyyy}[/]"), Justify.Center)
            };

            AnsiConsole.Write(panel);
            Console.WriteLine();
            AnsiConsole.MarkupLine("                 [blue]↑/↓/↩[/]");
        }
    }
}
