using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Members in groups and 1-1 conversations
/// </summary>
public class GroupMember : BaseEntity
{
    public int GroupId { get; set; }

    /// <summary>
    /// cognito_sub
    /// </summary>
    public string UserId { get; set; } = null!;

    public MemberRole Role { get; set; }

    /// <summary>
    /// Custom nickname in this group
    /// </summary>
    public string? Nickname { get; set; }

    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Last time user read messages in this group
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    /// <summary>
    /// User muted notifications
    /// </summary>
    public bool IsMuted { get; set; }

    /// <summary>
    /// FALSE if user left group
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When user left the group
    /// </summary>
    public DateTime? LeftAt { get; set; }

    // Navigation properties
    public Group Group { get; set; } = null!;
}
