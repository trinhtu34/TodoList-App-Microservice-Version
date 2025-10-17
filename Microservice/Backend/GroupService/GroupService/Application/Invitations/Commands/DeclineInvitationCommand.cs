using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Invitations.Commands;

public record DeclineInvitationCommand(int InvitationId, string UserId) : ICommand<bool>;

public class DeclineInvitationCommandHandler : IRequestHandler<DeclineInvitationCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public DeclineInvitationCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _context.GroupInvitations
            .FirstOrDefaultAsync(i => i.InvitationId == request.InvitationId, cancellationToken)
            ?? throw new Exception("Invitation not found");

        if (invitation.InvitedUser != request.UserId)
            throw new Exception("Not your invitation");

        if (invitation.Status != "pending")
            throw new Exception("Invitation already processed");

        invitation.Status = "declined";
        invitation.RespondedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
