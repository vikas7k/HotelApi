using HotelApi.Models;

namespace HotelApi.Service
{
    public interface IDataService
    {
        Task<List<Hotel>> GetHotelList();
        Task<Hotel?> GetHotel(string name);
        Task<Booking> CreateBooking(Booking booking);
        Task<int?> GetHotelId(string name);
        Task<RoomCapacity?> GetRoomCapacity(string type);
        Task<Booking?> GetValidBooking(BookingRequest bookingRequest, int hotelId, int roomTypeId);
        BookingDetails? GetBookingDetail(int bookingId);
        List<int> GetBookedRoomIds(DateOnly startDate, DateOnly endDate);
        List<RoomDetails> GetRoomsByType(string roomType);
        Task CreateData();
        Task ClearData();
    }
}
