using System;
using System.Collections.Generic;

namespace GroupService.Models;

/// <summary>
/// Quick lookup for existing 1-1 conversations between two users
/// </summary>
public partial class DirectMessageGroup
{
    /// <summary>
    /// Smaller cognito_sub (alphabetically)
    /// </summary>
    public string User1Id { get; set; } = null!;

    /// <summary>
    /// Larger cognito_sub (alphabetically)
    /// </summary>
    public string User2Id { get; set; } = null!;

    public int GroupId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual GroupsR Group { get; set; } = null!;
}
