using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Members.Queries;

public record GetGroupMembersQuery(int GroupId, string UserId) : IQuery<List<MemberResponse>>;

public class GetGroupMembersQueryHandler : IRequestHandler<GetGroupMembersQuery, List<MemberResponse>>
{
    private readonly GroupServiceDbContext _context;

    public GetGroupMembersQueryHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberResponse>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
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

        return members;
    }
}
