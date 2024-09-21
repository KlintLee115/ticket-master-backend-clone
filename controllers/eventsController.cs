using events_tickets_management_backend.Data;
using events_tickets_management_backend.Models;
using events_tickets_management_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace events_tickets_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController(EventService eventService) : ControllerBase()
    {
        private readonly EventService _eventService = eventService;
        private readonly Dictionary<string, int?> _frequencyCache = [];
        private readonly Dictionary<string, Event> _memoryCache = [];
        private const int VISIT_THRESHOLD_FOR_CACHE = 5;

        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents(int take = 5, int skip = 0, string? artistName = null,
            string? title = null, string? location = null, DateTime? begin_datetime = null, DateTime? end_datetime = null)
        {
            try
            {
                if (title != null && artistName != null && begin_datetime != null && end_datetime != null && location != null)

                {
                    var cacheKey = $"{begin_datetime}-{artistName}-{location}-{title}";

                    // Check cache
                    if (_memoryCache.TryGetValue(cacheKey, out Event? cachedEvent))
                    {

                        if (cachedEvent is Event e)
                        {
                            return Ok(new
                            {
                                eventDetail = cachedEvent,
                                cached = true
                            });
                        }

                        // Fetch the event if not in cache
                        Event? eventResult = await _eventService.GetEventAsync(title, artistName, begin_datetime.Value, end_datetime.Value, location);

                        // Update frequency count
                        if (_frequencyCache.TryGetValue(cacheKey, out int? value))
                        {
                            if (value.HasValue) _frequencyCache[cacheKey] = ++value;
                            else _frequencyCache[cacheKey] = 1;
                        }

                        if (eventResult != null)
                        {
                            // Cache the event if visit frequency exceeds threshold
                            if (_frequencyCache[cacheKey] >= VISIT_THRESHOLD_FOR_CACHE)
                            {
                                _memoryCache.Add(cacheKey, eventResult);
                            }
                        }

                        return Ok(eventResult);
                    }
                }

                var events = await _eventService.GetEventsAsync(title, artistName, begin_datetime, end_datetime, location, take, skip);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving events", error = ex.Message });
            }
        }

        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreateDto eventDto)
        {
            if (eventDto == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid event data.");
            }

            try
            {
                var createdEvent = await _eventService.CreateEvent(eventDto);
                return CreatedAtAction(nameof(CreateEvent), new {
                    createdEvent.Artist.Name,
                    createdEvent.Title,
                    createdEvent.BeginDatetime,
                    createdEvent.EndDatetime,
                    createdEvent.Location.Address
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating event", error = ex.Message });
            }
        }

        // PUT /api/events/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventUpdateDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid event data.");
            }

            try
            {
                var updatedEvent = await _eventService.UpdateEvent(id, eventDto);
                if (updatedEvent is FailServiceResponse response)
                {
                    return StatusCode(response.StatusCode, response.Message);
                }

                if (updatedEvent is SuccessServiceResponse sr)
                {
                    return Ok(new { sr.Data, message = "Event updated successfully" });
                }

                return StatusCode(500, new { message = "Error updating event" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating event", error = ex.Message });
            }
        }

        // DELETE /api/events?id=eventId
        [HttpDelete]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return Ok(new { message = "Event deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting event", error = ex.Message });
            }
        }
    }
}
