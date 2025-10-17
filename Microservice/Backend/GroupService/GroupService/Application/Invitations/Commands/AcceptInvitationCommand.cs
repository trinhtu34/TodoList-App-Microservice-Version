using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Invitations.Commands;

public record AcceptInvitationCommand(int InvitationId, string UserId) : ICommand<bool>;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public AcceptInvitationCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _context.GroupInvitations
            .FirstOrDefaultAsync(i => i.InvitationId == request.InvitationId, cancellationToken)
            ?? throw new Exception("Invitation not found");

        if (invitation.InvitedUser != request.UserId)
            throw new Exception("Not your invitation");

        if (invitation.Status != "pending")
            throw new Exception("Invitation already processed");

        if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt.Value < DateTime.UtcNow)
        {
            invitation.Status = "expired";
            await _context.SaveChangesAsync(cancellationToken);
            throw new Exception("Invitation expired");
        }

        // Add user to group
        var member = new GroupMember
        {
            GroupId = invitation.GroupId,
            UserId = request.UserId,
            Role = "member",
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.GroupMembers.Add(member);

        invitation.Status = "accepted";
        invitation.RespondedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
