namespace HotelApi.Models
{
    public class BookingRequest
    {
        public string HotelName { get; set; } = null!;

        public string RoomType { get; set; } = null!;

        public int Guests { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

    }
}
