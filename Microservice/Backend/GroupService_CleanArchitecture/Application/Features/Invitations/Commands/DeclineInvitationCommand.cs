using Application.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Invitations.Commands;

public record DeclineInvitationCommand(int InvitationId, string UserId) : ICommand;

public class DeclineInvitationCommandHandler : IRequestHandler<DeclineInvitationCommand>
{
    private readonly IGroupInvitationRepository _invitationRepository;

    public DeclineInvitationCommandHandler(IGroupInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken)
            ?? throw new KeyNotFoundException("Invitation not found");

        if (invitation.InvitedUser != request.UserId)
            throw new UnauthorizedAccessException("Not your invitation");

        if (invitation.Status != InvitationStatus.pending)
            throw new InvalidOperationException("Invitation already processed");

        invitation.Status = InvitationStatus.declined;
        invitation.RespondedAt = DateTime.UtcNow;

        await _invitationRepository.UpdateAsync(invitation, cancellationToken);
    }
}
