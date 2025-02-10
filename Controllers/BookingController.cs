using HotelApi.Models;
using HotelApi.Service;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;


namespace HotelApi.Controllers
{
    /// <summary>
    /// This service is used to search and book hotel bookings.
    /// </summary>
    /// <param name="dataService">Used to fetch database</param>
    /// <param name="logger">To log error</param>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(IDataService dataService, ILogger<BookingController> logger) : ControllerBase
    {
        private readonly IDataService _dataService = dataService;
        private readonly ILogger<BookingController> _logger = logger;

        /// <summary>
        /// To search booking by id.
        /// </summary>
        /// <param name="Id">Id of the booking to search.</param>
        /// <returns>Returns available booking details</returns>
        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            try
            {
                var booking = _dataService.GetBookingDetail(Id);
                if (booking == null)
                    return NotFound($"Booking {Id} not available");
                else
                    return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// To find available rooms for a given number of guests between start date and end date.
        /// </summary>
        /// <param name="guest">Number of guests for a booking.</param>
        /// <param name="startDate">Booking start date. Use date input format yyyy-mm-dd.</param>
        /// <param name="endDate">Booking end date. Use date input format yyyy-mm-dd.</param>
        /// <returns>Return all the available rooms to book between given dates.</returns>
        [HttpGet("Rooms/guest={guest}&startDate={startDate}&endDate={endDate}")]
        public IActionResult GetAvailableRooms(int guest, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("Start date is greater than end date");
                }
                var roomType = string.Empty;
                switch (guest)
                {
                    case 1:
                        roomType = "Single";
                        break;
                    case 2:
                        roomType = "Double";
                        break;
                    case 3:
                        roomType = "Deluxe";
                        break;
                    default:
                        roomType = string.Empty;
                        break;
                }
                var roomlist = _dataService.GetRoomsByType(roomType);

                var bookedRoomIds = _dataService.GetBookedRoomIds(startDate, endDate);

                if (roomlist.Any() && bookedRoomIds.Any())
                {
                    roomlist = roomlist.Where(r => !bookedRoomIds.Contains(r.Id)).ToList();
                }

                if (!roomlist.Any() && !string.IsNullOrEmpty(roomType))
                {
                    roomlist = _dataService.GetRoomsByType(string.Empty);
                    if (bookedRoomIds.Any())
                    {
                        roomlist = roomlist.Where(r => !bookedRoomIds.Contains(r.Id)).ToList();
                    }
                }

                if (roomlist != null && roomlist.Any())
                {
                    return Ok(roomlist);
                }
                else
                {
                    return Ok("Rooms not found for this request.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// To create a new booking
        /// </summary>
        /// <param name="bookingRequest">Please provide hotel name, room types (Single,Double or Deluxe), number of guests and start and end date in yyyy-mm-dd format.</param>
        /// <returns>Returns created booking id</returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Post(BookingRequest bookingRequest)
        {
            try
            {
                if (bookingRequest.StartDate > bookingRequest.EndDate)
                {
                    return BadRequest("Start date is greater than end date");
                }

                Booking? booking = new Booking();
                var hotelId = await _dataService.GetHotelId(bookingRequest.HotelName);
                if (!hotelId.HasValue)
                {
                    return BadRequest($"Hotel {bookingRequest.HotelName} is not valid");
                }

                var roomCapacity = await _dataService.GetRoomCapacity(bookingRequest.RoomType);
                if (roomCapacity == null)
                {
                    return BadRequest($"Room type {bookingRequest.RoomType} is not valid. Please provide: Single, Double or Deluxe.");
                }
                if (roomCapacity != null && roomCapacity.Capacity < bookingRequest.Guests)
                {
                    return BadRequest($"Room capacity {roomCapacity.Capacity} is less than number of guests {bookingRequest.Guests}");
                }
                if (hotelId.HasValue && roomCapacity != null)
                {
                    booking = await _dataService.GetValidBooking(bookingRequest, hotelId.Value, roomCapacity.Id);
                    if (booking != null)
                    {
                        await _dataService.CreateBooking(booking);
                    }
                }

                if (booking != null && booking.Id > 0)
                {
                    return Ok($"Booking {booking.Id} created successfully.");
                }
                else
                {
                    return Ok("Rooms not available for this date.Please try again.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

   
}
