using Application.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Invitations.Commands;

public record AcceptInvitationCommand(int InvitationId, string UserId) : ICommand;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand>
{
    private readonly IGroupInvitationRepository _invitationRepository;
    private readonly IGroupMemberRepository _memberRepository;

    public AcceptInvitationCommandHandler(IGroupInvitationRepository invitationRepository, IGroupMemberRepository memberRepository)
    {
        _invitationRepository = invitationRepository;
        _memberRepository = memberRepository;
    }

    public async Task Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken)
            ?? throw new KeyNotFoundException("Invitation not found");

        if (invitation.InvitedUser != request.UserId)
            throw new UnauthorizedAccessException("Not your invitation");

        if (invitation.Status != InvitationStatus.pending)
            throw new InvalidOperationException("Invitation already processed");

        if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt.Value < DateTime.UtcNow)
        {
            invitation.Status = InvitationStatus.expired;
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);
            throw new InvalidOperationException("Invitation expired");
        }

        // Add user to group
        var member = new GroupMember
        {
            GroupId = invitation.GroupId,
            UserId = request.UserId,
            Role = MemberRole.member,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _memberRepository.AddAsync(member, cancellationToken);

        invitation.Status = InvitationStatus.accepted;
        invitation.RespondedAt = DateTime.UtcNow;

        await _invitationRepository.UpdateAsync(invitation, cancellationToken);
    }
}
