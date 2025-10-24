using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Invitations.Queries;

public record GetUserInvitationsQuery(string UserId) : IQuery<List<InvitationResponse>>;

public class GetUserInvitationsQueryHandler : IRequestHandler<GetUserInvitationsQuery, List<InvitationResponse>>
{
    private readonly IGroupInvitationRepository _invitationRepository;

    public GetUserInvitationsQueryHandler(IGroupInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<List<InvitationResponse>> Handle(GetUserInvitationsQuery request, CancellationToken cancellationToken)
    {
        var invitations = await _invitationRepository.GetUserInvitationsAsync(request.UserId, cancellationToken);

        return invitations.Select(i => new InvitationResponse(
            i.InvitationId,
            i.GroupId,
            i.Group.GroupName ?? "Unnamed Group",
            i.InvitedBy,
            i.InvitedUser,
            i.Status.ToString().ToLower(),
            i.CreatedAt,
            i.ExpiresAt
        )).ToList();
    }
}
