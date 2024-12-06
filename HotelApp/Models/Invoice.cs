namespace HotelApp.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public Booking BookingInInvoice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsOverDue { get; set; } = false;
        public bool IsPaid { get; set; } = false;
    }

}
