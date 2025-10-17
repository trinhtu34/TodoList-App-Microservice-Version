namespace GroupService.DTOs;

public record CreateInvitationRequest(int GroupId, string InvitedUser);

public record InvitationResponse(
    int InvitationId,
    int GroupId,
    string GroupName,
    string InvitedBy,
    string InvitedUser,
    string Status,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);