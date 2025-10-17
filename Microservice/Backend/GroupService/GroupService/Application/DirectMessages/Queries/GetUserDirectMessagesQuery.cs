using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.DirectMessages.Queries;

public record GetUserDirectMessagesQuery(string UserId) : IQuery<List<DirectMessageResponse>>;

public class GetUserDirectMessagesQueryHandler : IRequestHandler<GetUserDirectMessagesQuery, List<DirectMessageResponse>>
{
    private readonly GroupServiceDbContext _context;

    public GetUserDirectMessagesQueryHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<List<DirectMessageResponse>> Handle(GetUserDirectMessagesQuery request, CancellationToken cancellationToken)
    {
        // Get all DMs where user is participant
        var dms = await _context.DirectMessageGroups
            .Where(dm => dm.User1Id == request.UserId || dm.User2Id == request.UserId)
            .Include(dm => dm.Group)
            .OrderByDescending(dm => dm.Group.LastMessageAt)
            .ToListAsync(cancellationToken);

        var responses = new List<DirectMessageResponse>();

        foreach (var dm in dms)
        {
            // Determine other user
            var otherUserId = dm.User1Id == request.UserId ? dm.User2Id : dm.User1Id;

            // Get current user's member info
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == dm.GroupId && m.UserId == request.UserId, cancellationToken);

            responses.Add(new DirectMessageResponse(
                dm.GroupId,
                otherUserId,
                dm.Group.LastMessageAt,
                member?.IsMuted ?? false
            ));
        }

        return responses;
    }
}
