using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Groups.Queries;

public record GetGroupByIdQuery(int GroupId, string UserId) : IQuery<GroupResponseGetGroupByID>;

public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupResponseGetGroupByID>
{
    private readonly GroupServiceDbContext _context;

    public GetGroupByIdQueryHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<GroupResponseGetGroupByID> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var isMember = group.GroupMembers.Any(m => m.UserId == request.UserId && m.IsActive == true);
        if (!isMember)
            throw new Exception("Not a member");

        var members = group.GroupMembers
            .Where(m => m.IsActive == true)
            .Select(m => new MemberResponse(
                m.UserId,
                m.Role!,
                m.Nickname,
                m.JoinedAt!.Value,
                m.IsMuted!.Value,
                m.IsActive!.Value
            ))
            .ToList();

        return new GroupResponseGetGroupByID(
            group.GroupId,
            group.GroupName,
            group.GroupAvatar,
            group.GroupDescription,
            group.GroupType!,
            group.CreatedBy,
            group.CreatedAt!.Value,
            group.LastMessageAt,
            group.IsActive!.Value,
            members.Count,
            members
        );
    }
}
