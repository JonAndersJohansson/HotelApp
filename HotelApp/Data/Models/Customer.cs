namespace HotelApp.Data.Models
{
    public enum TypeOfMembership
    {
        Brons,
        Silver,
        Guld
    }
    public class Customer
    {
        public int CustomerId { get; set; } // sätt till private set
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string EmailAddress { get; set; }
        public TypeOfMembership Membership { get; set; } = TypeOfMembership.Brons;
        public DateOnly? DateOfBirth { get; set; }
        public string? OtherInfoInCustomer { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Booking>? ListOfBookingsInCustomer { get; set; }

        public override string ToString()
        {
            return $"KundNr: {CustomerId}, {LastName} {FirstName}, {PhoneNumber}, {EmailAddress}, {Membership}-kund";
        }
    }

}
