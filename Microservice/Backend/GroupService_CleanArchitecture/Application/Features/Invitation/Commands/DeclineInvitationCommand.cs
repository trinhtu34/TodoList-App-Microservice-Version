using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Invitation.Commands;

public record DeclineInvitationCommand(int InvitationId, string UserId) : ICommand<bool>;

public class DeclineInvitationCommandHandler : IRequestHandler<DeclineInvitationCommand, bool>
{
    private readonly IGroupInvitationRepository _invitationRepository;
    public DeclineInvitationCommandHandler(IGroupInvitationRepository groupInvitationRepository)
    {
        _invitationRepository = groupInvitationRepository;
    }
    public async Task<bool> Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken)
            ?? throw new Exception("Invitation not found");

        if (invitation.InvitedUser != request.UserId)
            throw new Exception("Not your invitation");
        if(invitation.Status != InvitationStatus.pending)
            throw new Exception("Invitation already processed");

        invitation.Status = InvitationStatus.declined;
        invitation.RespondedAt = DateTime.UtcNow;

        await _invitationRepository.UpdateAsync(invitation);

        return true; 
    }
}
