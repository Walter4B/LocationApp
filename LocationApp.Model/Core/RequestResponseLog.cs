using System;
using System.Collections.Generic;

namespace LocationApp.Model.Core;

public partial class RequestResponseLog
{
    public int Id { get; set; }

    public string EndpointUrl { get; set; } = null!;

    public string RequestMethod { get; set; } = null!;

    public string? RequestHeaders { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseHeaders { get; set; }

    public string? ResponseBody { get; set; }

    public int? ResponseStatusCode { get; set; }

    public DateTime? CreateDate { get; set; }
}
