namespace HotelApi.Models
{
    public class RoomDetails
    {
        public int Id { get; set; }

        public string Hotel { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Room { get; set; } = null!;

        public string RoomType { get; set; } = null!;

        public int Capacity { get; set; }
    }
}
