namespace Application.DTOs;

public record CreateDirectMessageRequest(string OtherUserId);

public record DirectMessageResponse(
    int GroupId,
    string OtherUserId,
    DateTime? LastMessageAt,
    bool IsMuted
);
