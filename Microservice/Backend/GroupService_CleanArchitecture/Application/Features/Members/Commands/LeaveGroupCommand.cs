using Application.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Members.Commands;

public record LeaveGroupCommand(int GroupId, string UserId) : ICommand;

public class LeaveGroupCommandHandler : IRequestHandler<LeaveGroupCommand>
{
    private readonly IGroupMemberRepository _memberRepository;
    private readonly IGroupRepository _groupRepository;

    public LeaveGroupCommandHandler(IGroupMemberRepository memberRepository, IGroupRepository groupRepository)
    {
        _memberRepository = memberRepository;
        _groupRepository = groupRepository;
    }

    public async Task Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.GroupId, request.UserId, cancellationToken);
        
        if (member == null || !member.IsActive)
            throw new KeyNotFoundException("User is not a member of this group");

        // Check if user is member
        if (!await _groupRepository.IsUserMemberAsync(request.GroupId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("User is not a member of this group");

        // Soft delete by setting IsActive to false
        member.IsActive = false;
        member.LeftAt = DateTime.UtcNow;

        await _memberRepository.UpdateAsync(member, cancellationToken);
    }
}
