namespace HotelApp.Data.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; } // sätt till private set
        public required DateTime InvoiceDate { get; set; }
        public required decimal TotalAmount { get; set; }
        public required DateTime DueDate { get; set; }
        public bool IsOverDue { get; set; } = false;
        public required bool IsPaid { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public Booking BookingInInvoice { get; set; }
        public required int BookingId { get; set; }
        public override string ToString()
        {
            return $"FakNr: {InvoiceId}, BokNr: {BookingId}, Datum: {InvoiceDate:yy-mm-dd}, Belopp: {TotalAmount}, Betald = {(IsPaid ? "JA" : "NEJ")}";
        }
    }

}
