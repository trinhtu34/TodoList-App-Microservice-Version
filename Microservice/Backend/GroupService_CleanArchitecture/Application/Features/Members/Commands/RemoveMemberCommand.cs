using Application.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Members.Commands;

public record RemoveMemberCommand(int GroupId, string MemberUserId, string UserId) : ICommand;

public class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _memberRepository;

    public RemoveMemberCommandHandler(IGroupRepository groupRepository, IGroupMemberRepository memberRepository)
    {
        _groupRepository = groupRepository;
        _memberRepository = memberRepository;
    }

    public async Task Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new KeyNotFoundException("Group not found");

        var currentMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId && m.IsActive)
            ?? throw new UnauthorizedAccessException("Not a member");

        if (currentMember.Role != MemberRole.owner && currentMember.Role != MemberRole.admin)
            throw new UnauthorizedAccessException("No permission to remove members");

        var targetMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.MemberUserId && m.IsActive)
            ?? throw new KeyNotFoundException("Member not found");

        // Owner cannot be removed
        if (targetMember.Role == MemberRole.owner)
            throw new InvalidOperationException("Cannot remove owner");

        // Admin can only remove members, not other admins
        if (currentMember.Role == MemberRole.admin && targetMember.Role == MemberRole.admin)
            throw new UnauthorizedAccessException("Admin cannot remove other admins");

        targetMember.IsActive = false;
        targetMember.LeftAt = DateTime.UtcNow;

        await _memberRepository.UpdateAsync(targetMember, cancellationToken);
    }
}
