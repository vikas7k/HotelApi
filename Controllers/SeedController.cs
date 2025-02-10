using HotelApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HotelApi.Controllers
{
    /// <summary>
    /// Service to create and delete records in database
    /// </summary>
    /// <param name="dataService">Used to fetch database</param>
    /// <param name="logger">To log error</param>
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(IDataService dataService, ILogger<SeedController> logger) : ControllerBase
    {
        private readonly IDataService _dataService = dataService;
        private readonly ILogger<SeedController> _logger = logger;
        /// <summary>
        /// To create records in Hotels , Rooms and RoomTypes tables
        /// </summary>
        /// <returns></returns>
        [HttpPost("CreateRecords")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                //Delete old records before creating new
                await _dataService.ClearData();
                //Create new records
                await _dataService.CreateData();
                return Ok("Records are created in Hotels,Rooms and RoomTypes tables");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// To clear all records in Hotels,Rooms,RoomTypes and Bookings tables
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteRecords")]
        public async Task<IActionResult> ClearData()
        {
            try
            {
                await _dataService.ClearData();
                return Ok("All records are deleted in Hotels,Rooms,RoomTypes and Bookings tables");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
