using Microsoft.AspNetCore.Mvc;
using events_tickets_management_backend.Models;
using events_tickets_management_backend.Services;

namespace events_tickets_management_backend.controllers
{
    [ApiController]
    [Route("api/init")]
    public class InitController(InitService initService) : ControllerBase
    {
        private readonly InitService _initService = initService;

        /// <summary>
        /// Initialize seats for an event
        /// </summary>
        /// <param name="request">Request parameters</param>
        /// <returns>A response indicating the result of the operation</returns>
        [HttpPost("seats")]
        public async Task<IActionResult> CreateSeats([FromBody] CreateSeatsRequest request)
        {
            if (request.EventId <= 0 || request.NumberOfSeats <= 0 || request.AmtSections <= 0 || request.AmtRows <= 0)
            {
                return BadRequest("EventId, number of seats, number of sections, and number of rows are required.");
            }

            var result = await _initService.CreateTicketsForEvent(
                request.MeanPrice,
                request.SdPrice,
                request.EventId,
                request.NumberOfSeats,
                request.AmtSections,
                request.AmtRows
            );

            if (result is FailServiceResponse res)
            {
                return StatusCode(res.StatusCode, res.Message);
            }

            return CreatedAtAction(nameof(_initService.CreateTicketsForEvent), new { message = $"{request.NumberOfSeats} seats initialized for event {request.EventId}" });
        }

        /// <summary>
        /// Initialize artists
        /// </summary>
        [HttpPost("artists")]
        public async Task<IActionResult> CreateArtists()
        {
            var result = await _initService.CreateArtists();

            if (result is FailServiceResponse res) return StatusCode(res.StatusCode, res.Message);

            if (result is SuccessServiceResponse sr)
            {
                return CreatedAtAction(nameof(_initService.CreateArtists), new { sr.Data });
            }

            return StatusCode(500, new { message = "Error creating artists" });
        }

        /// <summary>
        /// Initialize events
        /// </summary>
        [HttpPost("events")]
        public async Task<IActionResult> CreateEvents()
        {
            var result = await _initService.CreateEvents();

            if (result is FailServiceResponse res) return StatusCode(res.StatusCode, res.Message);

            if (result is SuccessServiceResponse sr)
            {
                return CreatedAtAction(nameof(_initService.CreateEvents), new { sr.Data });
            }

            return StatusCode(500, new { message = "Error creating events" });
        }

        /// <summary>
        /// Initialize locations
        /// </summary>
        [HttpPost("locations")]
        public async Task<IActionResult> CreateLocations()
        {
            var result = await _initService.CreateLocations();

            if (result is FailServiceResponse res) return StatusCode(res.StatusCode, res.Message);

            if (result is SuccessServiceResponse sr)
            {
                return CreatedAtAction(nameof(_initService.CreateLocations), new { sr.Data });
            }

            return StatusCode(500, new { message = "Error creating locations" });
        }
    }

    public class CreateSeatsRequest
    {
        public int EventId { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal MeanPrice { get; set; }
        public decimal SdPrice { get; set; }
        public int AmtSections { get; set; }
        public int AmtRows { get; set; }
    }
}