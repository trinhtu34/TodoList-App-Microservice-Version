using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.DirectMessages.Queries;

public record GetUserDirectMessagesQuery(string UserId) : IQuery<List<DirectMessageResponse>>;

public class GetUserDirectMessagesQueryHandler : IRequestHandler<GetUserDirectMessagesQuery, List<DirectMessageResponse>>
{
    private readonly IDirectMessageRepository _directMessageRepository;
    private readonly IGroupMemberRepository _memberRepository;

    public GetUserDirectMessagesQueryHandler(IDirectMessageRepository directMessageRepository, IGroupMemberRepository memberRepository)
    {
        _directMessageRepository = directMessageRepository;
        _memberRepository = memberRepository;
    }

    public async Task<List<DirectMessageResponse>> Handle(GetUserDirectMessagesQuery request, CancellationToken cancellationToken)
    {
        // Get all DMs where user is participant
        var dms = await _directMessageRepository.GetUserDirectMessagesAsync(request.UserId, cancellationToken);

        var responses = new List<DirectMessageResponse>();

        foreach (var dm in dms)
        {
            // Determine other user
            var otherUserId = dm.User1Id == request.UserId ? dm.User2Id : dm.User1Id;

            // Get current user's member info
            var member = await _memberRepository.GetByIdAsync(dm.GroupId, request.UserId, cancellationToken);

            responses.Add(new DirectMessageResponse(
                dm.GroupId,
                otherUserId,
                dm.Group.LastMessageAt,
                member?.IsMuted ?? false
            ));
        }

        return responses;
    }
}
