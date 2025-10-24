namespace Application.DTOs;

public record MemberResponse(
    string UserId,
    string Role,
    string? Nickname,
    DateTime JoinedAt,
    bool IsMuted,
    bool IsActive
);

public record UpdateMemberRoleRequest(string Role);

public record UpdateMemberNicknameRequest(string Nickname);
