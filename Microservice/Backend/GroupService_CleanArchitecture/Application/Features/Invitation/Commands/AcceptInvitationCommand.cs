using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Invitation.Commands;

public record AcceptInvitationCommand(int InvitationId, string UserId)
    : ICommand<InvitationResponse>;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, InvitationResponse>
{
    private readonly IGroupMemberRepository _memberRepository;
    private readonly IGroupInvitationRepository _invitationRepository;
    private readonly IGroupRepository _groupRepository;

    public AcceptInvitationCommandHandler(
        IGroupMemberRepository memberRepository,
        IGroupInvitationRepository invitationRepository,    
        IGroupRepository groupRepository)
    {
        _memberRepository = memberRepository;
        _invitationRepository = invitationRepository;
        _groupRepository = groupRepository;
    }

    public async Task<InvitationResponse> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken)
            ?? throw new Exception("Invitation not found");

        if (invitation.InvitedUser != request.UserId)
            throw new Exception("Not your invitation");

        if (invitation.Status != InvitationStatus.pending)
            throw new Exception("Invitation already processed");

        if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt.Value < DateTime.UtcNow)
        {
            invitation.Status = InvitationStatus.expired;
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);
            throw new Exception("Invitation expired");
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

        var groupName = invitation.Group?.GroupName ?? "Unnamed Group";

        return new InvitationResponse(
            invitation.InvitationId,
            invitation.GroupId,
            groupName,
            invitation.InvitedBy,
            invitation.InvitedUser,
            invitation.Status.ToString().ToLower(),
            invitation.CreatedAt,
            invitation.ExpiresAt
        );
    }
}
