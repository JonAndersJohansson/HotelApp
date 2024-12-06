namespace HotelApp.Models
{
    public enum TypeOfMembership
    {
        Bronze,
        Silver,
        Gold
    }
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public TypeOfMembership Membership { get; set; }
        public List<Booking> ListOfBookingsInCustomer { get; set; }
    }

}
