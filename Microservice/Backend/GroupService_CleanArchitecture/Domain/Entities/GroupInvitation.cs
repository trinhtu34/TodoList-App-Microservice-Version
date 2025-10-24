using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Pending group invitations
/// </summary>
public class GroupInvitation : BaseEntity
{
    public int InvitationId { get; set; }

    public int GroupId { get; set; }

    /// <summary>
    /// cognito_sub
    /// </summary>
    public string InvitedBy { get; set; } = null!;

    /// <summary>
    /// cognito_sub or email
    /// </summary>
    public string InvitedUser { get; set; } = null!;

    public InvitationStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RespondedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public Group Group { get; set; } = null!;
}
