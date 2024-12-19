namespace HotelApp.Data.Models
{
    public class BookingRoom
    {
        public required int Id { get; set; }
        public required Booking Booking { get; set; }
        public required short RoomNumberAsID { get; set; }
        public required Room Room { get; set; }
    }
}
