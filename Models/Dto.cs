namespace events_tickets_management_backend.Models
{
    public class EventCreateDto
    {
        public required string Title { get; set; }
        public int ArtistId { get; set; }
        public required string Location { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public class EventUpdateDto
    {
        public required string Title { get; set; }
        public int ArtistId { get; set; }
    }
}