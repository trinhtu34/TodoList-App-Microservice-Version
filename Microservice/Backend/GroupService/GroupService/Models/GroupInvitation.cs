using System;
using System.Collections.Generic;

namespace GroupService.Models;

/// <summary>
/// Pending group invitations
/// </summary>
public partial class GroupInvitation
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

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? RespondedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public virtual GroupsR Group { get; set; } = null!;
}
