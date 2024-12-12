using HotelApp.UI;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.UI.Menus
{
    /// <summary>
    /// Klassen hanterar undermenyn InvoiceMenu
    /// </summary>
    public class InvoiceMenu : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly Lazy<ServiceMenu> _serviceMenu;

        public InvoiceMenu(Lazy<ServiceMenu> serviceMenu, DisplayList displayList)
        {
            _displayList = displayList;
            _serviceMenu = serviceMenu;
        }

        public readonly List<string> listInvoiceMenu = new List<string>
        {
        "Visa betalda fakturor", "Visa obetalda fakturor", "Visa förfallna fakturor", "Registrera betalning", "Ändra eller makulera" 
        };

        /// <summary>
        /// Metoden ger användaren alternativen i menyn genom DisplayList.
        /// </summary>
        public void MenuSwitch()
        {
            switch (_displayList.BrowseAList(listInvoiceMenu, false, Graphics.GetHeaderAsString("Meny Fakturor ↑/↓/↩"), true))
            {
                case 0:
                    //Visa betalda fakturor
                    break;
                case 1:
                    //Visa obetalda fakturor
                    break;
                case 2:
                    //Registrera en betalning på en befintlig faktura
                    break;
                case 3:
                    //Ändra eller makulera befintlig faktura
                    break;
                case 4:
                    //Visa fakturor som förfallit
                    break;
                case 5:
                    _serviceMenu.Value.MenuSwitch();
                    return;
                default:
                    Console.WriteLine("Ogiltigt alternativ 'InvoiceMenu', tryck valfri tangent för att återgå.");
                    Console.ReadKey();
                    _serviceMenu.Value.MenuSwitch();
                    break;
            }
        }
    }
}
