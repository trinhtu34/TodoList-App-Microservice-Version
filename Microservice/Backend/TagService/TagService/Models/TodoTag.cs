using System;
using System.Collections.Generic;

namespace TagService.Models;

public partial class TodoTag
{
    public int TodoId { get; set; }

    public int TagId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Tag Tag { get; set; } = null!;
}
