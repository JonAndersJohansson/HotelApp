using HotelApp.Controllers;
using HotelApp.Data.Models;
using HotelApp.Services.CustomerServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Services.InvoiceServices
{
    public class InvoicePropertySelector
    {
        private readonly InvoiceService _invoiceService;
        private readonly DisplayList _displayList;
        private readonly Lazy<InvoiceController>_invoiceController;
        public InvoicePropertySelector(Lazy<InvoiceController> invoiceController, DisplayList displayList, InvoiceService invoiceService)
        {
            _invoiceController = invoiceController;
            _displayList = displayList;
            _invoiceService = invoiceService;
        }
        public void PropertySwitch(Invoice invoice)
        {
            List<string> menuListInInvoicePropertySelector = new List<string>
            {
                "Betalningsvilkor (antal förfallodagar)", "Belopp på faktura *", "Betald / Obetald *", "Pågånde / Förfallen", "Koppla faktura mot bokningsnummer *", "Kontrollera & Spara"
            };
            while (true)
            {
                string messageToUseInHeader = "Ny faktura. Välj i listan och lägg till fakturauppgifter ↑/↓/↩ (* = Krav)";

                switch (_displayList.BrowseAList(menuListInInvoicePropertySelector, false, Graphics.GetHeaderAsString(messageToUseInHeader), true))
                {
                    case 0:
                        //förfallodagar
                        _invoiceService.GetDueDate(invoice);
                        break;
                    case 1:
                        //Belopp på faktura
                        _invoiceService.GetTotalAmount(invoice);
                        break;
                    case 2:
                        //Betald / Obetald
                        _invoiceService.GetIsPaid(invoice);
                        break;
                    case 3:
                        //Pågånde / Förfallen
                        _invoiceService.GetIsOverDue(invoice);
                        break;
                    case 4:
                        //koppla till bokning
                        _invoiceService.GetBookingNr(invoice);
                        break;
                    case 5:
                        if (_invoiceService.ValidateInvoice(invoice) == true)
                        {
                            _invoiceService.AddInvoice(invoice);
                            _invoiceController.Value.MenuSwitch();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n  Ogiltigt värde, var vänlig fyll i alla obligatoriska uppgifter.\n  Tryck valfri tangent för att försöka igen...");
                            Console.ReadKey();
                            break;
                        }
                    case 6:
                        _invoiceController.Value.MenuSwitch();
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ i InvoicePropertySelector switch, tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        _invoiceController.Value.MenuSwitch();
                        break;
                }
            }
        }
    }
}
