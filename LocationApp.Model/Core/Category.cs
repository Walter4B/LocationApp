using System;
using System.Collections.Generic;

namespace LocationApp.Model.Core;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
