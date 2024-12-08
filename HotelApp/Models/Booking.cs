namespace HotelApp.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public Customer CustomerInBooking { get; set; }
        public short RoomNumber { get; set; }
        public List<Room> ListOfRoomsInBooking { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte NumberOfGuests { get; set; }
        public bool IsCurrentlyStaying { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public Invoice InvoiceInBooking { get; set; }

        public override string ToString()
        {
            return $"BookingId: {BookingId}, Customer: {CustomerInBooking.LastName}, RoomNr: {RoomNumber}, StartDate: {StartDate:yyyy-MM-dd}, EndDate: {EndDate:yyyy-MM-dd}, IsCancelled: {IsCancelled}";
        }
    }

}
