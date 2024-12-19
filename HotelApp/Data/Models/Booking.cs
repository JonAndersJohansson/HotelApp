namespace HotelApp.Data.Models
{
    public class Booking
    {
        public int Id { get; set; } // sätt till private set
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required byte NumberOfGuests { get; set; }
        public bool IsCancelled { get; set; } = false;
        public string? OtherInfoInBooking { get; set; }
        public required Customer CustomerInBooking { get; set; }
        public required int CustomerId { get; set; }

        public List<BookingRoom> ListOfBookingRoomsInBooking { get; set; } = new List<BookingRoom>();
        public List<Invoice> ListOfInvoicesInBooking { get; set; } = new List<Invoice>();

        public override string ToString()
        {
            return $"Id: {Id}, Namn: {CustomerInBooking.FirstName} {CustomerInBooking.LastName} Antal Gäster: {NumberOfGuests} StartDate: {StartDate:yyyy-MM-dd}, EndDate: {EndDate:yyyy-MM-dd}, IsCancelled: {IsCancelled}";
        }
    }

}
