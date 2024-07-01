using System;
using System.Collections.Generic;

namespace ReactAPI.Models;

public partial class Cake
{
    public int Id { get; set; }

    public string? Name { get; set; } = null!;

    public string? Origin { get; set; } = null!;

    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
