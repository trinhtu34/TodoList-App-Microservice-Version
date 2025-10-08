using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Groups.Commands.Group;

public record DeleteGroupCommand(int GroupId, string UserId) : ICommand<bool>;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public DeleteGroupCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var member = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId)
            ?? throw new Exception("Not a member");

        if (member.Role != "owner")
            throw new Exception("Only owner can delete group");

        _context.GroupsRs.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
