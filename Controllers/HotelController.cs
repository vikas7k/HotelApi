using HotelApi.Models;
using HotelApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Controllers
{
    /// <summary>
    /// This service is used to display hotels records
    /// </summary>
    /// <param name="dataService">Used to fetch database</param>
    /// <param name="logger">To log error</param>
    [ApiController]
    [Route("[controller]")]
    public class HotelController(IDataService dataService, ILogger<HotelController> logger) : ControllerBase
    {
        private readonly IDataService _dataService = dataService;
        private readonly ILogger<HotelController> _logger = logger;

        /// <summary>
        /// To display all hotels records
        /// </summary>
        /// <returns>Returns list of all hotels available</returns>
        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _dataService.GetHotelList();

                return hotels == null || !hotels.Any() ? NotFound($"Hotels not found") : Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// To search a hotel by it's name (wild card search)
        /// </summary>
        /// <param name="name">The name of hotel to search.This is a wild card search.</param>
        /// <returns>Returns hotel records</returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetHotelByName(string name)
        {
            try
            {
                var hotel = await _dataService.GetHotel(name);
                return hotel == null ? NotFound($"{name} not found") : Ok(hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
