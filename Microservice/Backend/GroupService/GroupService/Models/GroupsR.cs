using System;
using System.Collections.Generic;

namespace GroupService.Models;

/// <summary>
/// Stores both 1-1 conversations and group chats
/// </summary>
public partial class GroupsR
{
    public int GroupId { get; set; }

    /// <summary>
    /// NULL for 1-1 chats or unnamed groups
    /// </summary>
    public string? GroupName { get; set; }

    public string? GroupAvatar { get; set; }

    /// <summary>
    /// direct=1-1 chat, group=multiple users
    /// </summary>
    public string? GroupType { get; set; }

    /// <summary>
    /// cognito_sub
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// For sorting by recent activity
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual DirectMessageGroup? DirectMessageGroup { get; set; }

    public virtual ICollection<GroupInvitation> GroupInvitations { get; set; } = new List<GroupInvitation>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
