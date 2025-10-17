using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Invitations.Queries;

public record GetUserInvitationsQuery(string UserId) : IQuery<List<InvitationResponse>>;

public class GetUserInvitationsQueryHandler : IRequestHandler<GetUserInvitationsQuery, List<InvitationResponse>>
{
    private readonly GroupServiceDbContext _context;

    public GetUserInvitationsQueryHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvitationResponse>> Handle(GetUserInvitationsQuery request, CancellationToken cancellationToken)
    {
        var invitations = await _context.GroupInvitations
            .Where(i => i.InvitedUser == request.UserId && i.Status == "pending")
            .Include(i => i.Group)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvitationResponse(
                i.InvitationId,
                i.GroupId,
                i.Group.GroupName ?? "Unnamed Group",
                i.InvitedBy,
                i.InvitedUser,
                i.Status!,
                i.CreatedAt!.Value,
                i.ExpiresAt
            ))
            .ToListAsync(cancellationToken);

        return invitations;
    }
}
