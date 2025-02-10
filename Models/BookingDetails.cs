namespace HotelApi.Models
{
    public class BookingDetails
    {
        public int Id { get; set; }

        public string Hotel { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Room { get; set; } = null!;

        public string RoomType  { get; set; } = null!;

        public int Guests { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public DateOnly CreatedDate { get; set; }
    }
}
