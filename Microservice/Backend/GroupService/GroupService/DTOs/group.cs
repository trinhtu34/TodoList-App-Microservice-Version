namespace GroupService.DTOs;

public record CreateGroupRequest(string GroupName, string? GroupAvatar, string? GroupDescription);

public record UpdateGroupRequest(string? GroupName, string? GroupAvatar, string? GroupDescription);

public record GroupResponse(
    int GroupId,
    string? GroupName,
    string? GroupAvatar,
    string? GroupDescription,
    string GroupType,
    string CreatedBy,
    DateTime CreatedAt,
    DateTime? LastMessageAt,
    bool IsActive,
    int MemberCount
);

public record GroupListResponse(
    int GroupId,
    string? GroupName,
    string? GroupAvatar,
    string GroupType,
    DateTime? LastMessageAt,
    int UnreadCount
);
