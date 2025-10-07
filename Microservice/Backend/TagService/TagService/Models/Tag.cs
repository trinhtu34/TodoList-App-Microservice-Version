using System;
using System.Collections.Generic;

namespace TagService.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    /// <summary>
    /// Creator
    /// </summary>
    public string CognitoSub { get; set; } = null!;

    /// <summary>
    /// NULL = personal tag, NOT NULL = group tag
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// Hex color
    /// </summary>
    public string? Color { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
}
