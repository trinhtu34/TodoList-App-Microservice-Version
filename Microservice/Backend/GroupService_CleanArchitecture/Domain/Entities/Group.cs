using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Stores both 1-1 conversations and group chats
/// </summary>
public class Group : BaseEntity
{
    public int GroupId { get; set; }

    /// <summary>
    /// NULL for 1-1 chats or unnamed groups
    /// </summary>
    public string? GroupName { get; set; }

    public string? GroupAvatar { get; set; }

    /// <summary>
    /// Group description/bio
    /// </summary>
    public string? GroupDescription { get; set; }

    /// <summary>
    /// direct=1-1 chat, group=multiple users
    /// </summary>
    public GroupType GroupType { get; set; }

    /// <summary>
    /// cognito_sub
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// For sorting by recent activity
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    public bool IsActive { get; set; }

    // Navigation properties
    public DirectMessageGroup? DirectMessageGroup { get; set; }
    public ICollection<GroupInvitation> GroupInvitations { get; set; } = new List<GroupInvitation>();
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
