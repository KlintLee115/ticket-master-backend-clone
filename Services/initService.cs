using Microsoft.EntityFrameworkCore;
using events_tickets_management_backend.Models;
using events_tickets_management_backend.Data;
using events_tickets_management_backend.utils;

namespace events_tickets_management_backend.Services
{
    public class InitService(DataContext context)
    {
        private readonly DataContext _context = context;

        public async Task<ServiceResponse> CreateTicketsForEvent(decimal meanPrice, decimal sd, int eventId, int numberOfSeats, int amountOfSections, int amountOfRows)
        {
            try
            {
                await CreatePartitionIfNotExists(eventId);

                var ticketData = new List<Ticket>();
                for (int i = 0; i < amountOfSections; i++)
                {
                    for (int j = 0; j < amountOfRows; j++)
                    {
                        for (int k = 0; k < numberOfSeats; k++)
                        {
                            ticketData.Add(new Ticket
                            {
                                EventId = eventId,
                                Price = GeneratePrice(meanPrice, sd),
                                SectionNumber = i,
                                RowNumber = j,
                                SeatNumber = k,
                            });
                        }
                    }
                }

                await _context.Tickets.AddRangeAsync(ticketData);
                await _context.SaveChangesAsync();

                return new SuccessServiceResponse { };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new FailServiceResponse { StatusCode = 500, Message = "Failed to create tickets" };
            }
        }

        public async Task<ServiceResponse> CreateLocations()
        {
            try
            {
                var locations = new List<string>
            {
                "Event Hall, New York, NY, USA",
                // ... (other locations)
                "Forest National, Brussels, Belgium"
            };

                var locationEntities = locations.Select(address => new Location { Address = address }).ToList();
                await _context.Locations.AddRangeAsync(locationEntities);
                await _context.SaveChangesAsync();

                return new SuccessServiceResponse { };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new FailServiceResponse { StatusCode = 500, Message = "Failed to create locations" };
            }
        }

        public async Task<ServiceResponse> CreateArtists()
        {
            try
            {
                var names = new List<string>
            {
                "Drake",
                // ... (other artist names)
                "Migos"
            };

                var artistEntities = names.Select(name => new Artist { Name = name }).ToList();
                await _context.Artists.AddRangeAsync(artistEntities);
                await _context.SaveChangesAsync();

                return new SuccessServiceResponse { };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new FailServiceResponse { StatusCode = 500, Message = "Failed to create artists" };
            }
        }

        public async Task<ServiceResponse> CreateEvents()
        {
            try
            {
                var events = new List<Event>();

                for (int i = 0; i < 100; i++)
                {
                    var artist = await GetRandomArtist();
                    var beginDateTime = GetRandomDateTime(new DateTime(2024, 1, 1), new DateTime(2024, 12, 30));
                    var endDateTime = GetRandomDateTime(beginDateTime, new DateTime(2024, 12, 31));

                    var randomLocation = await GetRandomLocation();

                    var newEvent = new Event
                    {
                        Title = $"Event {i + 1} featuring {artist.Name}",
                        ArtistId = artist.Id,
                        BeginDatetime = beginDateTime,
                        EndDatetime = endDateTime,
                        LocationId = randomLocation.Id,
                    };

                    events.Add(newEvent);
                }

                await _context.Events.AddRangeAsync(events);
                await _context.SaveChangesAsync();

                return new SuccessServiceResponse { };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new FailServiceResponse { StatusCode = 500, Message = "Failed to create events" };
            }
        }

        private async Task CreatePartitionIfNotExists(int eventId)
        {

            var partitionName = $"tickets_eventid{eventId}";

            string mutation = Consts.CreatePartitionIfNotExistsSQL(partitionName, eventId);

            await _context.Database.ExecuteSqlRawAsync(mutation);
        }

        private static decimal GeneratePrice(decimal meanPrice, decimal sd)
        {
            var minPrice = meanPrice - sd;
            var maxPrice = meanPrice + sd;
            return new Random().Next((int)minPrice, (int)maxPrice + 1);
        }

        private static DateTime GetRandomDateTime(DateTime start, DateTime end)
        {
            var randomTime = start.AddMilliseconds(new Random().Next(0, (int)(end - start).TotalMilliseconds));
            return randomTime;
        }

        private async Task<Location> GetRandomLocation()
        {
            var count = await _context.Locations.CountAsync();
            var randomIndex = new Random().Next(0, count);
            return await _context.Locations.Skip(randomIndex).Take(1).SingleAsync();
        }

        private async Task<Artist> GetRandomArtist()
        {
            var count = await _context.Artists.CountAsync();
            var randomIndex = new Random().Next(0, count);
            return await _context.Artists.Skip(randomIndex).Take(1).SingleAsync();
        }
    }
}