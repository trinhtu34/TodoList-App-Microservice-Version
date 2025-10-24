using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Members.Queries;

public record GetGroupMembersQuery(int GroupId, string UserId) : IQuery<List<MemberResponse>>;

public class GetGroupMembersQueryHandler : IRequestHandler<GetGroupMembersQuery, List<MemberResponse>>
{
    private readonly IGroupMemberRepository _memberRepository;
    private readonly IGroupRepository _groupRepository;

    public GetGroupMembersQueryHandler(IGroupMemberRepository memberRepository, IGroupRepository groupRepository)
    {
        _memberRepository = memberRepository;
        _groupRepository = groupRepository;
    }

    public async Task<List<MemberResponse>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
    {
        // Check if user is member of the group
        if (!await _groupRepository.IsUserMemberAsync(request.GroupId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("User is not a member of this group");

        var members = await _memberRepository.GetGroupMembersAsync(request.GroupId, cancellationToken);

        return members.Select(m => new MemberResponse(
            m.UserId,
            m.Role.ToString().ToLower(),
            m.Nickname,
            m.JoinedAt,
            m.IsMuted,
            m.IsActive
        )).ToList();
    }
}
