using System;
using System.Collections.Generic;

namespace events_tickets_management_backend.Models;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int ArtistId { get; set; }

    public DateTime BeginDatetime { get; set; }

    public DateTime EndDatetime { get; set; }

    public int LocationId { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;
}
