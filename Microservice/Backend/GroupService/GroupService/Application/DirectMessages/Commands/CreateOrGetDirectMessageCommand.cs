using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.DirectMessages.Commands;

public record CreateOrGetDirectMessageCommand(string UserId, string OtherUserId) : ICommand<DirectMessageResponse>;

public class CreateOrGetDirectMessageCommandHandler : IRequestHandler<CreateOrGetDirectMessageCommand, DirectMessageResponse>
{
    private readonly GroupServiceDbContext _context;

    public CreateOrGetDirectMessageCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<DirectMessageResponse> Handle(CreateOrGetDirectMessageCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == request.OtherUserId)
            throw new Exception("Cannot create DM with yourself");

        // Sort user IDs alphabetically
        var user1 = string.Compare(request.UserId, request.OtherUserId) < 0 ? request.UserId : request.OtherUserId;
        var user2 = string.Compare(request.UserId, request.OtherUserId) < 0 ? request.OtherUserId : request.UserId;

        // Check if DM already exists
        var existingDm = await _context.DirectMessageGroups
            .Include(dm => dm.Group)
            .FirstOrDefaultAsync(dm => dm.User1Id == user1 && dm.User2Id == user2, cancellationToken);

        if (existingDm != null)
        {
            // Get current user's member info
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == existingDm.GroupId && m.UserId == request.UserId, cancellationToken);

            return new DirectMessageResponse(
                existingDm.GroupId,
                request.OtherUserId,
                existingDm.Group.LastMessageAt,
                member?.IsMuted ?? false
            );
        }

        // Create new DM group
        var group = new GroupsR
        {
            GroupName = null,
            GroupType = "direct",
            CreatedBy = request.UserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.GroupsRs.Add(group);
        await _context.SaveChangesAsync(cancellationToken);

        // Create direct message mapping
        var dmMapping = new DirectMessageGroup
        {
            User1Id = user1,
            User2Id = user2,
            GroupId = group.GroupId,
            CreatedAt = DateTime.UtcNow
        };

        _context.DirectMessageGroups.Add(dmMapping);

        // Add both users as members
        _context.GroupMembers.AddRange(
            new GroupMember
            {
                GroupId = group.GroupId,
                UserId = request.UserId,
                Role = "member",
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            },
            new GroupMember
            {
                GroupId = group.GroupId,
                UserId = request.OtherUserId,
                Role = "member",
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            }
        );

        await _context.SaveChangesAsync(cancellationToken);

        return new DirectMessageResponse(
            group.GroupId,
            request.OtherUserId,
            null,
            false
        );
    }
}
