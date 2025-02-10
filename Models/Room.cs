using System;
using System.Collections.Generic;

namespace HotelApi.Models;

public partial class Room
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public int HotelId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual RoomType Type { get; set; } = null!;
}
