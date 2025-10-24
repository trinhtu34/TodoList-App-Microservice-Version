using Application.Common;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.DirectMessages.Commands;

public record CreateOrGetDirectMessageCommand(string UserId, string OtherUserId) : ICommand<DirectMessageResponse>;

public class CreateOrGetDirectMessageCommandHandler : IRequestHandler<CreateOrGetDirectMessageCommand, DirectMessageResponse>
{
    private readonly IDirectMessageRepository _directMessageRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _memberRepository;

    public CreateOrGetDirectMessageCommandHandler(
        IDirectMessageRepository directMessageRepository,
        IGroupRepository groupRepository,
        IGroupMemberRepository memberRepository)
    {
        _directMessageRepository = directMessageRepository;
        _groupRepository = groupRepository;
        _memberRepository = memberRepository;
    }

    public async Task<DirectMessageResponse> Handle(CreateOrGetDirectMessageCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == request.OtherUserId)
            throw new InvalidOperationException("Cannot create DM with yourself");

        // Sort user IDs alphabetically
        var user1 = string.Compare(request.UserId, request.OtherUserId) < 0 ? request.UserId : request.OtherUserId;
        var user2 = string.Compare(request.UserId, request.OtherUserId) < 0 ? request.OtherUserId : request.UserId;

        // Check if DM already exists
        var existingDm = await _directMessageRepository.GetByUsersAsync(user1, user2, cancellationToken);

        if (existingDm != null)
        {
            // Get current user's member info
            var member = await _memberRepository.GetByIdAsync(existingDm.GroupId, request.UserId, cancellationToken);

            return new DirectMessageResponse(
                existingDm.GroupId,
                request.OtherUserId,
                existingDm.Group.LastMessageAt,
                member?.IsMuted ?? false
            );
        }

        // Create new DM group
        var group = new Group
        {
            GroupName = null,
            GroupType = GroupType.direct,
            CreatedBy = request.UserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _groupRepository.AddAsync(group, cancellationToken);

        // Create direct message mapping
        var dmMapping = new DirectMessageGroup
        {
            User1Id = user1,
            User2Id = user2,
            GroupId = group.GroupId,
            CreatedAt = DateTime.UtcNow
        };

        await _directMessageRepository.AddAsync(dmMapping, cancellationToken);

        // Add both users as members
        var members = new[]
        {
            new GroupMember
            {
                GroupId = group.GroupId,
                UserId = request.UserId,
                Role = MemberRole.member,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            },
            new GroupMember
            {
                GroupId = group.GroupId,
                UserId = request.OtherUserId,
                Role = MemberRole.member,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        foreach (var member in members)
        {
            await _memberRepository.AddAsync(member, cancellationToken);
        }

        return new DirectMessageResponse(
            group.GroupId,
            request.OtherUserId,
            null,
            false
        );
    }
}
