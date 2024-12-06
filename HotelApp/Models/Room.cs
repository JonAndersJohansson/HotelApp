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
        public int RoomId { get; set; }
        public short RoomNumber { get; set; }
        public BedSize RoomType { get; set; }
        public float RoomSizeInSquareMeters { get; set; }
        public byte MaxPersonsAllowedInRoom { get; set; }
        public byte NumberOfPossibleExtraBeds { get; set; }
        public float CostPerNight { get; set; }
        public List<Booking> ListOfBooking { get; set; } = new List<Booking>();
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return $"Room {RoomNumber}: {RoomType}, {CostPerNight}";
        }
    }

}
