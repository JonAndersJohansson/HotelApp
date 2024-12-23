using System.Xml.Linq;

namespace HotelApp.Data.Models
{
    public enum BedSize
    {
        Single,
        Double
    }
    public class Room
    {
        public int Id { get; set; } // sätt till private set
        public required short RoomNumber { get; set; }
        public required BedSize RoomType { get; set; } = BedSize.Single;
        public byte NumberOfPossibleExtraBeds { get; set; } = 0;
        public required decimal CostPerNight { get; set; }
        public bool IsDisabilityFriendly { get; set; } = false;
        public string? OtherOrDescription { get; set; }
        public bool IsActive { get; set; } = true;
        public List<BookingRoom>? ListOfBookingRoomsInRoom { get; set; } = new List<BookingRoom>();

        public override string ToString()
        {
            return $"{RoomNumber} - {RoomType}, Pris: {CostPerNight}, Möjliga extrasängar: {NumberOfPossibleExtraBeds} - {(IsActive ? "AKTIV" : "AVSTÄNGD")}";
        }
    }
}
