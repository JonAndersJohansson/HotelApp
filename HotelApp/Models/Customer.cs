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
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string EmailAddress { get; set; }
        public TypeOfMembership Membership { get; set; } = TypeOfMembership.Bronze;
        public DateOnly? DateOfBirth { get; set; }
        public List<Booking>? ListOfBookingsInCustomer { get; set; }
    }

}
