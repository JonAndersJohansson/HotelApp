namespace HotelApp.Data.Models
{
    public class Booking
    {
        public int BookingId { get; set; } // sätt till private set
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required byte NumberOfGuests { get; set; }
        public bool IsCancelled { get; set; } = false;
        public string? OtherInfoInBooking { get; set; }
        public required Customer CustomerInBooking { get; set; }
        public required int CustomerId { get; set; }
        //public Invoice? InvoiceInBooking { get; set; }
        //public int? InvoiceId { get; set; }
        public List<BookingRoom> ListOfBookingRoomsInBooking { get; set; } = new List<BookingRoom>();
        public List<Invoice> ListOfInvoicesInBooking { get; set; } = new List<Invoice>();

        public override string ToString()
        {
            return $"BookingId: {BookingId}, Namn: {CustomerInBooking.FirstName} {CustomerInBooking.LastName} Antal Gäster: {NumberOfGuests} StartDate: {StartDate:yyyy-MM-dd}, EndDate: {EndDate:yyyy-MM-dd}, IsCancelled: {IsCancelled}";
        }
    }

}
