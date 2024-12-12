using System.Xml.Linq;

namespace HotelApp.Models
{
    public enum BedSize
    {
        Single,
        Double
    }
    public class Room
    {
        public required short RoomNumberAsID { get; set; }
        public required BedSize RoomType { get; set; } = BedSize.Single;
        public byte NumberOfPossibleExtraBeds { get; set; } = 0;
        public required decimal CostPerNight { get; set; }
        public bool IsDisabilityFriendly { get; set; } = false;
        public string? OtherOrDescription { get; set; }
        public bool IsActive { get; set; } = true;
        public List<BookingRoom>? ListOfBookingRoomsInRoom { get; set; } = new List<BookingRoom>();

        public override string ToString()
        {
            return $"{RoomNumberAsID} - {RoomType}, Pris: {CostPerNight}, Möjliga extrasängar: {NumberOfPossibleExtraBeds}, AKTIV = {(IsActive ? "Ja" : "Nej")}";
        }
    }

}
