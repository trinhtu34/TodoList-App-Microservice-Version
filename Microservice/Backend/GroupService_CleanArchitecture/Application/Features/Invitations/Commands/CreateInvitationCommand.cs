using Application.Common;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Invitations.Commands;

public record CreateInvitationCommand(int GroupId, string InvitedUser, string UserId) 
    : ICommand<InvitationResponse>;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, InvitationResponse>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupInvitationRepository _invitationRepository;

    public CreateInvitationCommandHandler(IGroupRepository groupRepository, IGroupInvitationRepository invitationRepository)
    {
        _groupRepository = groupRepository;
        _invitationRepository = invitationRepository;
    }

    public async Task<InvitationResponse> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new KeyNotFoundException("Group not found");

        var member = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId && m.IsActive)
            ?? throw new UnauthorizedAccessException("Not a member");

        if (member.Role != MemberRole.owner && member.Role != MemberRole.admin)
            throw new UnauthorizedAccessException("No permission to invite users");

        // Check if already member
        var alreadyMember = group.GroupMembers.Any(m => m.UserId == request.InvitedUser && m.IsActive);
        if (alreadyMember)
            throw new InvalidOperationException("User is already a member");

        // Check existing pending invitation
        var existingInvitations = await _invitationRepository.GetUserInvitationsAsync(request.InvitedUser, cancellationToken);
        var existingInvite = existingInvitations.FirstOrDefault(i => i.GroupId == request.GroupId && i.Status == InvitationStatus.pending);

        if (existingInvite != null)
            throw new InvalidOperationException("Invitation already sent");

        var invitation = new GroupInvitation
        {
            GroupId = request.GroupId,
            InvitedBy = request.UserId,
            InvitedUser = request.InvitedUser,
            Status = InvitationStatus.pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return new InvitationResponse(
            invitation.InvitationId,
            invitation.GroupId,
            group.GroupName ?? "Unnamed Group",
            invitation.InvitedBy,
            invitation.InvitedUser,
            invitation.Status.ToString().ToLower(),
            invitation.CreatedAt,
            invitation.ExpiresAt
        );
    }
}
