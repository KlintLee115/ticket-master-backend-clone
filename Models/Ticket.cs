namespace events_tickets_management_backend.Models;

public partial class Ticket
{
    public int EventId { get; set; }

    public int RowNumber { get; set; }

    public int SectionNumber { get; set; }

    public int SeatNumber { get; set; }

    public decimal Price { get; set; }

    public bool Isbought { get; set; }
    public DateTime? PurchasedTime { get; set; }

    public string? Buyeremail { get; set; }
}
