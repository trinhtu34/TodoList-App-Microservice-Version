using Application.Common;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Groups.Commands;

public record CreateGroupCommand(string GroupName, string? GroupAvatar, string? GroupDescription, string UserId) 
    : ICommand<GroupResponse>;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupResponse>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _memberRepository;

    public CreateGroupCommandHandler(IGroupRepository groupRepository, IGroupMemberRepository memberRepository)
    {
        _groupRepository = groupRepository;
        _memberRepository = memberRepository;
    }

    public async Task<GroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = new Group
        {   
            GroupName = request.GroupName,
            GroupAvatar = request.GroupAvatar,
            GroupDescription = request.GroupDescription,
            GroupType = GroupType.group,
            CreatedBy = request.UserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _groupRepository.AddAsync(group, cancellationToken);

        // Add creator as owner
        var member = new GroupMember
        {
            GroupId = group.GroupId,
            UserId = request.UserId,
            Role = MemberRole.owner,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _memberRepository.AddAsync(member, cancellationToken);

        return new GroupResponse(
            group.GroupId,
            group.GroupName,
            group.GroupAvatar,
            group.GroupDescription,
            group.GroupType.ToString().ToLower(),
            group.CreatedBy,
            group.CreatedAt,
            group.LastMessageAt,
            group.IsActive,
            1
        );
    }
}
