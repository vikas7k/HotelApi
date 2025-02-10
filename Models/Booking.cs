using System;
using System.Collections.Generic;

namespace HotelApi.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int Guests { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateOnly CreatedDate { get; set; }

    public virtual Room Room { get; set; } = null!;
}
