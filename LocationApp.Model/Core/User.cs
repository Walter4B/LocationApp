using System;
using System.Collections.Generic;

namespace LocationApp.Model.Core;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? ApiKey { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
