using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Groups.Commands;

public record ArchiveGroupCommand(int GroupId, string UserId) : ICommand<bool>;

public class ArchiveGroupCommandHandler : IRequestHandler<ArchiveGroupCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public ArchiveGroupCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ArchiveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var member = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId)
            ?? throw new Exception("Not a member");

        if (member.Role != "owner" && member.Role != "admin")
            throw new Exception("No permission");

        group.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
