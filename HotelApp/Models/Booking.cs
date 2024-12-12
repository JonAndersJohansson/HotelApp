namespace HotelApp.Models
{
    public class Booking
    {
        public required int BookingId { get; set; }
        public required Customer CustomerInBooking { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required byte NumberOfGuests { get; set; }
        public bool IsCancelled { get; set; } = false;
        public Invoice? InvoiceInBooking { get; set; }
        public List<BookingRoom> ListOfBookingRoomsInBooking { get; set; } = new List<BookingRoom>();

        public override string ToString()
        {
            return $"BookingId: {BookingId}, Customer: {CustomerInBooking.LastName}, StartDate: {StartDate:yyyy-MM-dd}, EndDate: {EndDate:yyyy-MM-dd}, IsCancelled: {IsCancelled}";
        }
    }

}
