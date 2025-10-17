using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Members.Commands;

public record LeaveGroupCommand(int GroupId, string UserId) : ICommand<bool>;

public class LeaveGroupCommandHandler : IRequestHandler<LeaveGroupCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public LeaveGroupCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var member = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId && m.IsActive == true)
            ?? throw new Exception("Not a member");

        // Owner cannot leave if there are other members
        if (member.Role == "owner")
        {
            var otherMembers = group.GroupMembers.Count(m => m.IsActive == true && m.UserId != request.UserId);
            if (otherMembers > 0)
                throw new Exception("Owner must transfer ownership or remove all members before leaving");
        }

        member.IsActive = false;
        member.LeftAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
