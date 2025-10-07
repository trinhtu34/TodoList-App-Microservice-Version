using System;
using System.Collections.Generic;

namespace GroupService.Models;

/// <summary>
/// Members in groups and 1-1 conversations
/// </summary>
public partial class GroupMember
{
    public int GroupId { get; set; }

    /// <summary>
    /// cognito_sub
    /// </summary>
    public string UserId { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? JoinedAt { get; set; }

    /// <summary>
    /// Last time user read messages in this group
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    /// <summary>
    /// User muted notifications
    /// </summary>
    public bool? IsMuted { get; set; }

    public virtual GroupsR Group { get; set; } = null!;
}
