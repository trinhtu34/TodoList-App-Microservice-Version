using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Members.Commands;

public record UpdateMemberRoleCommand(int GroupId, string MemberUserId, string NewRole, string UserId) : ICommand<bool>;

public class UpdateMemberRoleCommandHandler : IRequestHandler<UpdateMemberRoleCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public UpdateMemberRoleCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.NewRole != "owner" && request.NewRole != "admin" && request.NewRole != "member")
            throw new Exception("Invalid role");

        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var currentMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId && m.IsActive == true)
            ?? throw new Exception("Not a member");

        if (currentMember.Role != "owner")
            throw new Exception("Only owner can change roles");

        var targetMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.MemberUserId && m.IsActive == true)
            ?? throw new Exception("Member not found");

        // Transfer ownership
        if (request.NewRole == "owner")
        {
            currentMember.Role = "admin";
            targetMember.Role = "owner";
        }
        else
        {
            targetMember.Role = request.NewRole;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
