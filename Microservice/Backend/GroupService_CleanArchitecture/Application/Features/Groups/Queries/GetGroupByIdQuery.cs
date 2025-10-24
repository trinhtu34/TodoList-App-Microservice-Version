using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Groups.Queries;

public record GetGroupByIdQuery(int GroupId, string UserId) : IQuery<GroupResponseGetGroupByID>;

public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupResponseGetGroupByID>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupByIdQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupResponseGetGroupByID> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken);
        
        if (group == null)
            throw new KeyNotFoundException($"Group with ID {request.GroupId} not found");

        // Check if user is member
        if (!await _groupRepository.IsUserMemberAsync(request.GroupId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("User is not a member of this group");

        var members = group.GroupMembers
            .Where(m => m.IsActive)
            .Select(m => new MemberResponse(
                m.UserId,
                m.Role.ToString().ToLower(),
                m.Nickname,
                m.JoinedAt,
                m.IsMuted,
                m.IsActive
            )).ToList();

        return new GroupResponseGetGroupByID(
            group.GroupId,
            group.GroupName,
            group.GroupAvatar,
            group.GroupDescription,
            group.GroupType.ToString().ToLower(),
            group.CreatedBy,
            group.CreatedAt,
            group.LastMessageAt,
            group.IsActive,
            members.Count,
            members
        );
    }
}
