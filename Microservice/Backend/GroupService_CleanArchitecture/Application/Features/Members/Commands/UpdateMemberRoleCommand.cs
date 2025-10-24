using Application.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Members.Commands;

public record UpdateMemberRoleCommand(int GroupId, string MemberUserId, string NewRole, string UserId) : ICommand;

public class UpdateMemberRoleCommandHandler : IRequestHandler<UpdateMemberRoleCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _memberRepository;

    public UpdateMemberRoleCommandHandler(IGroupRepository groupRepository, IGroupMemberRepository memberRepository)
    {
        _groupRepository = groupRepository;
        _memberRepository = memberRepository;
    }

    public async Task Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<MemberRole>(request.NewRole, true, out var newRole))
            throw new ArgumentException("Invalid role");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new KeyNotFoundException("Group not found");

        var currentMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId && m.IsActive)
            ?? throw new UnauthorizedAccessException("Not a member");

        if (currentMember.Role != MemberRole.owner)
            throw new UnauthorizedAccessException("Only owner can change roles");

        var targetMember = group.GroupMembers.FirstOrDefault(m => m.UserId == request.MemberUserId && m.IsActive)
            ?? throw new KeyNotFoundException("Member not found");

        // Transfer ownership
        if (newRole == MemberRole.owner)
        {
            currentMember.Role = MemberRole.admin;
            await _memberRepository.UpdateAsync(currentMember, cancellationToken);
            
            targetMember.Role = MemberRole.owner;
        }
        else
        {
            targetMember.Role = newRole;
        }

        await _memberRepository.UpdateAsync(targetMember, cancellationToken);
    }
}
