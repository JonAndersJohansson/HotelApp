namespace HotelApp.Models
{
    public class Customer
    {
        public int CustomerId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public string EmailAddress { get; private set; }
        public List<Booking> Bookings { get; private set; } = new List<Booking>();
    }

}
