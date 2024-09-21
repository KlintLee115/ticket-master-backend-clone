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

                return new SuccessServiceResponse {Data = ticketData };
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
                "Madison Square Garden, New York, NY, USA",
    "Wembley Stadium, London, UK",
    "Sydney Opera House, Sydney, Australia",
    "Staples Center, Los Angeles, CA, USA",
    "Royal Albert Hall, London, UK",
    "Red Rocks Amphitheatre, Morrison, CO, USA",
    "Tokyo Dome, Tokyo, Japan",
    "O2 Arena, London, UK",
    "MGM Grand Garden Arena, Las Vegas, NV, USA",
    "Barclays Center, Brooklyn, NY, USA",
    "AccorHotels Arena, Paris, France",
    "Mercedes-Benz Arena, Berlin, Germany",
    "Rogers Arena, Vancouver, BC, Canada",
    "Forest National, Brussels, Belgium",
            "Radio City Music Hall, New York, NY, USA",
    "The Forum, Inglewood, CA, USA",
    "Rod Laver Arena, Melbourne, Australia",
    "Olympic Stadium, Seoul, South Korea",
    "Bercy Arena, Paris, France",
    "SSE Hydro, Glasgow, Scotland",
    "Amway Center, Orlando, FL, USA",
    "Scotiabank Arena, Toronto, ON, Canada",
    "Sportpaleis, Antwerp, Belgium",
    "Ziggo Dome, Amsterdam, Netherlands",
    "Estadio Azteca, Mexico City, Mexico",
    "Allianz Arena, Munich, Germany",
    "Maracanã Stadium, Rio de Janeiro, Brazil",
    "U Arena, Nanterre, France",
    "Arena di Verona, Verona, Italy"
            };

                var locationEntities = locations.Select(address => new Location { Address = address }).ToList();
                await _context.Locations.AddRangeAsync(locationEntities);
                await _context.SaveChangesAsync();

                return new SuccessServiceResponse { Data = locationEntities };
            }
            catch (Exception ex)
            {
                return new FailServiceResponse { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse> CreateArtists()
        {
            try
            {
                var names = new List<string>
                {
                "Drake",
    "Taylor Swift",
    "Beyoncé",
    "Ed Sheeran",
    "Adele",
    "Kendrick Lamar",
    "Billie Eilish",
    "The Weeknd",
    "Bruno Mars",
    "Post Malone",
    "Ariana Grande",
    "Harry Styles",
    "Lady Gaga",
    "Justin Bieber",
    "Dua Lipa",
    "Rihanna",
    "Cardi B",
    "Kanye West",
    "Shawn Mendes",
    "Travis Scott",
    "Migos",
    "Lizzo",
    "Doja Cat",
    "SZA",
    "Bad Bunny",
    "J. Cole",
    "Lana Del Rey",
    "H.E.R.",
    "Olivia Rodrigo",
    "Lil Nas X"
                };

                var artistEntities = names.Select(name => new Artist { Name = name }).ToList();
                await _context.Artists.AddRangeAsync(artistEntities);
                await _context.SaveChangesAsync();

                return new SuccessServiceResponse {Data = artistEntities };
            }
            catch (Exception ex)
            {
                return new FailServiceResponse { StatusCode = 500, Message = ex.Message };
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

                return new SuccessServiceResponse { Data = events };
            }
            catch (Exception ex)
            {
                return new FailServiceResponse { StatusCode = 500, Message = ex.Message };
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