using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Groups.Queries;

public record GetUserGroupsQuery(string UserId) : IQuery<List<GroupListResponse>>;

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, List<GroupListResponse>>
{
    private readonly GroupServiceDbContext _context;

    public GetUserGroupsQueryHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<List<GroupListResponse>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _context.GroupMembers
            .Where(m => m.UserId == request.UserId && m.IsActive == true)
            .Include(m => m.Group)
            .Select(m => new GroupListResponse(
                m.Group.GroupId,
                m.Group.GroupName,
                m.Group.GroupAvatar,
                m.Group.GroupType!,
                m.Group.LastMessageAt,
                0 // TODO: Calculate unread count
            ))
            .OrderByDescending(g => g.LastMessageAt)
            .ToListAsync(cancellationToken);

        return groups;
    }
}
