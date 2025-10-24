using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Groups.Queries;

public record GetUserGroupsQuery(string UserId) : IQuery<List<GroupListResponse>>;

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, List<GroupListResponse>>
{
    private readonly IGroupRepository _groupRepository;

    public GetUserGroupsQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<List<GroupListResponse>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _groupRepository.GetUserGroupsAsync(request.UserId, cancellationToken);

        return groups.Select(g => new GroupListResponse(
            g.GroupId,
            g.GroupName,
            g.GroupAvatar,
            g.GroupType.ToString().ToLower(),
            g.LastMessageAt,
            0, // TODO: Calculate unread count
            g.GroupMembers.First(m => m.UserId == request.UserId).Role.ToString().ToLower(),
            g.GroupMembers.Count(gm => gm.IsActive)
        )).ToList();
    }
}
