using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Invitations.Commands;

public record CreateInvitationCommand(int GroupId, string InvitedUser, string UserId) 
    : ICommand<InvitationResponse>;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, InvitationResponse>
{
    private readonly GroupServiceDbContext _context;

    public CreateInvitationCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<InvitationResponse> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var member = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId && m.IsActive == true)
            ?? throw new Exception("Not a member");

        if (member.Role != "owner" && member.Role != "admin")
            throw new Exception("No permission");

        // Check if already member
        var alreadyMember = group.GroupMembers.Any(m => m.UserId == request.InvitedUser && m.IsActive == true);
        if (alreadyMember)
            throw new Exception("User is already a member");

        // Check existing pending invitation
        var existingInvite = await _context.GroupInvitations
            .FirstOrDefaultAsync(i => i.GroupId == request.GroupId 
                && i.InvitedUser == request.InvitedUser 
                && i.Status == "pending", cancellationToken);

        if (existingInvite != null)
            throw new Exception("Invitation already sent");

        var invitation = new GroupInvitation
        {
            GroupId = request.GroupId,
            InvitedBy = request.UserId,
            InvitedUser = request.InvitedUser,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.GroupInvitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);

        return new InvitationResponse(
            invitation.InvitationId,
            invitation.GroupId,
            group.GroupName ?? "Unnamed Group",
            invitation.InvitedBy,
            invitation.InvitedUser,
            invitation.Status!,
            invitation.CreatedAt!.Value,
            invitation.ExpiresAt
        );
    }
}
