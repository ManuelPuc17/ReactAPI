using System;
using System.Collections.Generic;

namespace ReactAPI.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CakeId { get; set; }

    public double Flavor { get; set; }

    public double Presentation { get; set; }

    public virtual Cake? Cake { get; set; }

    public virtual User? User { get; set; }
}
