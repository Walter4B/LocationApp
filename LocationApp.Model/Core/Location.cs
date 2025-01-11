using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace LocationApp.Model.Core;

public partial class Location
{
    public int Id { get; set; }

    public string? ExternalPlaceId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public decimal? Rating { get; set; }

    public int? RatingsTotal { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Geometry? GeoLocation { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
