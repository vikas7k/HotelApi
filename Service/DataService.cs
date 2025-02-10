using HotelApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace HotelApi.Service
{
    public class DataService : IDataService
    {
        private readonly HotelBookingContext _context;

        public DataService(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task<List<Hotel>> GetHotelList()
        {
            return await _context.Hotels.ToListAsync();
        }


        public async Task<Hotel?> GetHotel(string name)
        {
            return await _context.Hotels
                  .FirstOrDefaultAsync(h => h.Name.Contains(name));
        }

        public async Task<Booking> CreateBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking?> GetValidBooking(BookingRequest bookingRequest, int hotelId, int roomTypeId)
        {
            var roomList = await _context.Rooms.Where(r => r.HotelId == hotelId && r.TypeId == roomTypeId).ToListAsync();
            var bookedRoomIds = GetBookedRoomIds(bookingRequest.StartDate, bookingRequest.EndDate);
            if (roomList.Any() && bookedRoomIds.Any())
            {
                roomList = roomList.Where(r => !bookedRoomIds.Contains(r.Id)).ToList();
            }
            if (roomList.Any())
            {
                return new Booking
                {
                    RoomId = roomList.First().Id,
                    Guests = bookingRequest.Guests,
                    StartDate = bookingRequest.StartDate,
                    EndDate = bookingRequest.EndDate
                };
            }

            return null;
        }
        
        public async Task<int?> GetHotelId(string name)
        {
            var hotel = await _context.Hotels
                 .FirstOrDefaultAsync(h => h.Name.ToLower().Equals(name.ToLower()));

            return hotel == null ? null : hotel.Id;
        }
        public async Task<RoomCapacity?> GetRoomCapacity(string type)
        {
            var room = await _context.RoomTypes
                 .FirstOrDefaultAsync(r => r.Type.ToLower().Equals(type.ToLower()));

            return room == null ? null : new RoomCapacity { Id= room.Id , Capacity = room.Capacity};
        }

        public List<RoomDetails> GetRoomsByType(string roomType)
        {
            var roomList =  (from r in _context.Rooms 
                                  join h in _context.Hotels on r.HotelId equals h.Id
                                  join rt in _context.RoomTypes on r.TypeId equals rt.Id
                                  where rt.Type.Equals((string.IsNullOrEmpty(roomType) ? rt.Type : roomType))
                                  select new RoomDetails
                                  {      
                                      Id = r.Id,
                                      Hotel = h.Name,
                                      Phone = h.Phone,
                                      City = h.City,
                                      Room = r.Name,
                                      RoomType = rt.Type,
                                      Capacity= rt.Capacity,                                    
                                  }).ToList();
            return roomList;

        }


        public List<int> GetBookedRoomIds(DateOnly startDate, DateOnly endDate)
        {
            var bookedRoomIds = (from b in _context.Bookings
                                 where ((b.StartDate <= startDate && b.EndDate >= startDate) ||
                                        (b.StartDate < endDate && b.EndDate >= endDate) ||
                                        (startDate <= b.StartDate && endDate >= b.StartDate))
                                 select b.RoomId ).ToList<int>();
            return bookedRoomIds;
        }

        public BookingDetails? GetBookingDetail(int bookingId)
        {
            var bookingDetails = (from b in _context.Bookings                             
                                  join r in _context.Rooms on b.RoomId equals r.Id
                                  join h in _context.Hotels on r.HotelId equals h.Id
                                  join rt in _context.RoomTypes on r.TypeId equals rt.Id
                                  where b.Id == bookingId
                                  select new BookingDetails
                                   {
                                     Id = bookingId,
                                     Hotel = h.Name,
                                     Phone = h.Phone,
                                     City = h.City,
                                     Room = r.Name,
                                     RoomType = rt.Type,
                                     Guests= b.Guests,
                                     StartDate = b.StartDate,
                                     EndDate = b.EndDate,
                                     CreatedDate = b.CreatedDate
                                   }).FirstOrDefault();

            return bookingDetails;
        }
        public async Task CreateData()
        {
            _context.Hotels.Add(new Hotel { Name = "ParkRoyal", City = "London", Phone = "034343434" });
            _context.Hotels.Add(new Hotel { Name = "Victoria", City = "Reading", Phone = "0456787565" });
            _context.Hotels.Add(new Hotel { Name = "Cresent", City = "Leeds", Phone = "0343434687" });
            _context.Hotels.Add(new Hotel { Name = "HolidayHome", City = "Bournemouth", Phone = "043576775" });
            _context.RoomTypes.Add(new RoomType { Type = "Single", Capacity = 1 });
            _context.RoomTypes.Add(new RoomType { Type = "Double", Capacity = 2 });
            _context.RoomTypes.Add(new RoomType { Type = "Deluxe", Capacity = 3 });
            await _context.SaveChangesAsync();

            var roomTypes = await _context.RoomTypes.ToListAsync();

            var singleRoomType = roomTypes.Where(r => r.Type.Equals("Single")).Single().Id;
            var doubleRoomType = roomTypes.Where(r => r.Type.Equals("Double")).Single().Id;
            var deluxRoomType = roomTypes.Where(r => r.Type.Equals("Deluxe")).Single().Id;

            var hotelList = await _context.Hotels.ToListAsync();

            hotelList.ForEach(h =>
            {
                h.Rooms.Add(
                    new Room
                    {
                        Name = $"{h.Name}Single1",
                        HotelId = h.Id,
                        TypeId = singleRoomType
                    });
                h.Rooms.Add(
                    new Room
                    {
                        Name = $"{h.Name}Single2",
                        HotelId = h.Id,
                        TypeId = singleRoomType
                    });
                h.Rooms.Add(
                   new Room
                   {
                       Name = $"{h.Name}Double1",
                       HotelId = h.Id,
                       TypeId = doubleRoomType
                   });
                h.Rooms.Add(
                    new Room
                    {
                        Name = $"{h.Name}Double2",
                        HotelId = h.Id,
                        TypeId = doubleRoomType
                    });
                h.Rooms.Add(
                    new Room
                    {
                        Name = $"{h.Name}Deluxe1",
                        HotelId = h.Id,
                        TypeId = deluxRoomType
                    });
                h.Rooms.Add(
                    new Room
                    {
                        Name = $"{h.Name}Deluxe2",
                        HotelId = h.Id,
                        TypeId = deluxRoomType
                    });
            });

            await _context.SaveChangesAsync();
        }

        public async Task ClearData()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Bookings];DBCC CHECKIDENT ('[Bookings]', RESEED, 0);");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Rooms];DBCC CHECKIDENT ('[Rooms]', RESEED, 0);");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [RoomTypes];DBCC CHECKIDENT ('[RoomTypes]', RESEED, 0);");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Hotels];DBCC CHECKIDENT ('[Hotels]', RESEED, 0);");
        }
    }
}
