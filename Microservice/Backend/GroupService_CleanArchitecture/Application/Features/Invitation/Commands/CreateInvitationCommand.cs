using Application.Common;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using System.Threading;

namespace Application.Features.Invitation.Commands;

public record CreateInvitationCommand(int GroupId, string InvitedUser, string UserId)
    : ICommand<InvitationResponse>;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, InvitationResponse>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupInvitationRepository _invitationRepository;
    public CreateInvitationCommandHandler(IGroupRepository groupRepository, IGroupInvitationRepository invitationRepository )
    {
        _groupRepository = groupRepository;
        _invitationRepository = invitationRepository;
    }
    public async Task<InvitationResponse> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group == null) { 
            throw new KeyNotFoundException($"Group with ID {request.GroupId} not found");
        }

        // check inviter
        var member = await _groupRepository.IsUserMemberAsync(request.GroupId, request.UserId, cancellationToken);
        if (!member)
        {
            throw new UnauthorizedAccessException("Inviter is not a member of this group");
        }
        var isOwnerOrAdmin = await _groupRepository.IsUserOwnerOrAdminAsync(request.GroupId, request.UserId, cancellationToken);
        if (!isOwnerOrAdmin)
        {
            throw new UnauthorizedAccessException("Only owners and admins can invite users to the group");
        }

        // check invited user
        var alreadyMember = await _groupRepository.IsUserMemberAsync(request.GroupId, request.InvitedUser, cancellationToken);
        if (!alreadyMember) {
            throw new InvalidOperationException("Invited user is already a member of this group");
        }
        // check existing pending invitation
        var existingInvitation = await _invitationRepository.CheckInvitationExists(request.GroupId, request.InvitedUser, cancellationToken);
        if (!existingInvitation)
        {
            throw new InvalidOperationException("Invitation already sent");
        }
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
