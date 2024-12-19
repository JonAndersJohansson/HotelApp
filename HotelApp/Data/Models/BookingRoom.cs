namespace HotelApp.Data.Models
{
    public class BookingRoom
    {
        public int Id { get; set; }
        public required int BookingId { get; set; }
        public required Booking Booking { get; set; }
        public required int RoomId { get; set; }
        public required Room Room { get; set; }
    }
}
