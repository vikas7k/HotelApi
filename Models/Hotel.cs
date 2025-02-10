using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HotelApi.Models;

public partial class Hotel
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
