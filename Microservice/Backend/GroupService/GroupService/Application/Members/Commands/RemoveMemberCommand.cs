using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Members.Commands;

public record RemoveMemberCommand(int GroupId, string MemberUserId, string UserId) : ICommand<bool>;

public class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public RemoveMemberCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var currentMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId && m.IsActive == true)
            ?? throw new Exception("Not a member");

        if (currentMember.Role != "owner" && currentMember.Role != "admin")
            throw new Exception("No permission");

        var targetMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.MemberUserId && m.IsActive == true)
            ?? throw new Exception("Member not found");

        // Owner cannot be removed
        if (targetMember.Role == "owner")
            throw new Exception("Cannot remove owner");

        // Admin can only remove members, not other admins
        if (currentMember.Role == "admin" && targetMember.Role == "admin")
            throw new Exception("Admin cannot remove other admins");

        targetMember.IsActive = false;
        targetMember.LeftAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
