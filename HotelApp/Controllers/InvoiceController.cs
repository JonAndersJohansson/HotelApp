using HotelApp.Data.Models;
using HotelApp.Services.InvoiceServices;
using HotelApp.UI;
using HotelApp.UI.Menus;
using HotelApp.Utilities;

namespace HotelApp.Controllers
{

    public class InvoiceController : IMenu
    {
        private readonly DisplayList _displayList;
        private readonly InvoiceService _invoiceService;

        public InvoiceController(DisplayList displayList, InvoiceService invoiceService)
        {
            _displayList = displayList;
            _invoiceService = invoiceService;
        }
        
        public void MenuSwitch()
        {
            List<string> listInvoiceMenu = new List<string>
            {
                "Sök & Visa en faktura", "Visa 100 senaste betalda fakturorna (och välj en)", "Visa 100 senaste obetalda fakturorna (och välj en)", "Visa 100 senaste förfallna fakturorna (och välj en)", "Sök & registrera betalning på befintlig faktura", "Sök & annulera faktura"
            };
            while (true)
            {
                switch (_displayList.BrowseAList(listInvoiceMenu, false, Graphics.GetHeaderAsString("Meny Fakturor ↑/↓/↩"), true))
                {
                    case 0:
                        _invoiceService.SearchInvoiceToList(false, false);
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
                        _invoiceService.SearchInvoiceToList(false, true);
                        break;
                    case 5:
                        _invoiceService.SearchInvoiceToList(true, false);
                        break;
                    case 6:
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
