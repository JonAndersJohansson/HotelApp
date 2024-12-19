using HotelApp.Services;
using HotelApp.Data.Models;
using HotelApp.Services.InvoiceServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Controllers
{

    public class InvoiceController : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly InvoiceService _invoiceService;
        private readonly Lazy<InvoicePropertySelector> _invoicePropertySelector;


        public InvoiceController(DisplayList displayList, InvoiceService invoiceService, Lazy<InvoicePropertySelector> invoicePropertySelector)
        {
            _displayList = displayList;
            _invoiceService = invoiceService;
            _invoicePropertySelector = invoicePropertySelector;
        }
        
        public void MenuSwitch()
        {
            List<string> listInvoiceMenu = new List<string>
            {
                "Sök & Visa en faktura", "Visa 100 senaste betalda fakturorna (och välj en)", "Visa 100 senaste obetalda fakturorna (och välj en)", "Visa 100 senaste förfallna fakturorna (och välj en)", "Skapa en faktura (och koppla till bokning)", "Sök & registrera betalning på befintlig faktura", "Sök & annulera faktura"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(listInvoiceMenu, false, Graphics.GetHeaderAsString("Meny Fakturor ↑/↓/↩"), true))
                {
                    case 0:
                        _invoiceService.GetListOfInvoiceBySearch(false, false);
                        break;
                    case 1:
                        _invoiceService.GetAInvoiceFrom100IsPaid(true);
                        break;
                    case 2:
                        _invoiceService.GetAInvoiceFrom100IsPaid(false);
                        break;
                    case 3:
                        _invoiceService.GetAInvoiceFrom100IsOverDue();
                        break;
                    case 4:
                        var newInvoice = new Invoice { InvoiceDate = DateTime.MinValue, TotalAmount = 0, IsPaid = false, BookingId = 0, DueDate = DateTime.MinValue };
                        _invoicePropertySelector.Value.PropertySwitch(newInvoice);
                        break;
                    case 5:
                        _invoiceService.GetListOfInvoiceBySearch(false, true);
                        break;
                    case 6:
                        _invoiceService.GetListOfInvoiceBySearch(true, false);
                        break;
                    case 7:
                        return;
                    default:
                        Console.WriteLine("Ogiltigt alternativ 'InvoiceMenu', tryck valfri tangent för att återgå.");
                        Console.ReadKey();
                        return;
                }
            }
        }
    }
}
