using events_tickets_management_backend.Data;
using events_tickets_management_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace events_tickets_management_backend.Services
{
    public class EventService(DataContext context)
    {
        private readonly DataContext _context = context;

        public async Task<List<Event>> GetEventsAsync(string? title, string? artistName, DateTime? beginDateTime, DateTime? endDateTime, string? location, int take = 5, int skip = 0)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(e => e.Title.Contains(title));

            if (!string.IsNullOrEmpty(artistName))
                query = query.Where(e => e.Artist.Name.Contains(artistName));

            if (!string.IsNullOrEmpty(location))
                query = query.Where(e => e.Location.Address.Contains(location));

            if (beginDateTime.HasValue)
                query = query.Where(e => e.BeginDatetime >= beginDateTime.Value);

            if (endDateTime.HasValue)
                query = query.Where(e => e.BeginDatetime <= endDateTime.Value);

            return await query
                .OrderBy(e => e.BeginDatetime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Event?> GetEventAsync(string title, string artistName, DateTime beginDateTime, DateTime endDateTime, string location)
        {
            var events = await _context.Events
                .Include(e => e.Artist)
                .Include(e => e.Location)
                .Where(e => e.Title == title &&
                            e.Artist.Name == artistName &&
                            e.Location.Address == location &&
                            e.BeginDatetime == beginDateTime &&
                            e.EndDatetime == endDateTime)
                .ToListAsync();

            if (events.Count > 1)
                throw new Exception("More than 1 event with the same information");

            return events.FirstOrDefault();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Artist)
                .Include(e => e.Location)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> CreateEventAsync(EventCreateDto eventDto)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Address == eventDto.Location) ?? throw new Exception("Location not found");
            var eventEntity = new Event
            {
                Title = eventDto.Title,
                ArtistId = eventDto.ArtistId,
                BeginDatetime = eventDto.BeginDateTime,
                EndDatetime = eventDto.EndDateTime,
                LocationId = location.Id
            };

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<ServiceResponse> UpdateEvent(int id, EventUpdateDto eventDto)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return new FailServiceResponse
                {
                    Message = "Event not found",
                    StatusCode = 404
                };
            }

            if (string.IsNullOrEmpty(eventDto.Title) && eventDto.ArtistId == 0)
            {
                return new FailServiceResponse
                {
                    Message = "Nothing to update",
                    StatusCode = 204
                };
            }

            if (!string.IsNullOrEmpty(eventDto.Title))
            {
                eventEntity.Title = eventDto.Title;
            }

            if (eventDto.ArtistId != 0)
            {
                var artist = await _context.Artists.FindAsync(eventDto.ArtistId);
                if (artist == null)
                {
                    return new FailServiceResponse
                    {
                        Message = "Artist not found",
                        StatusCode = 404
                    };
                }

                eventEntity.ArtistId = eventDto.ArtistId;
            }

            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();

            return new SuccessServiceResponse
            {
                Message = "Event updated successfully"
            };
        }

        public async Task<ServiceResponse> DeleteEventAsync(int eventId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return new FailServiceResponse
                {
                    Message = "Event not found",
                    StatusCode = 404
                };
            }

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();

            return new SuccessServiceResponse
            {
                Message = "Event deleted successfully"
            };
        }
    }
}