﻿using System;
using System.Collections.Generic;

namespace events_tickets_management_backend.Models;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
