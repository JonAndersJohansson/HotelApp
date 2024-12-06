namespace HotelApp.Models
{
    public class Booking
    {
        public int BookingId { get; private set; }
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public int RoomId { get; private set; }
        public Room Room { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsCancelled { get; private set; }
        public Invoice Invoice { get; private set; }
    }

}
