namespace HotelApp.Models
{
    public class Invoice
    {
        public int InvoiceId { get; private set; }
        public int BookingId { get; private set; }
        public Booking Booking { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsOverDue { get; private set; }
        public bool IsPaid { get; private set; }
    }

}
