namespace HotelApi.Models
{
    public class RoomRequest
    {
        public int Guest { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
   
    }
}
