using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Groups.Commands.Group;

public record CreateGroupCommand(string GroupName, string? GroupAvatar, string? GroupDescription, string UserId) 
    : ICommand<GroupResponse>;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupResponse>
{
    private readonly GroupServiceDbContext _context;

    public CreateGroupCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<GroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = new GroupsR
        {
            GroupName = request.GroupName,
            GroupAvatar = request.GroupAvatar,
            GroupDescription = request.GroupDescription,
            GroupType = "group",
            CreatedBy = request.UserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.GroupsRs.Add(group);
        await _context.SaveChangesAsync(cancellationToken);

        // Add creator as owner
        var member = new GroupMember
        {
            GroupId = group.GroupId,
            UserId = request.UserId,
            Role = "owner",
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        return new GroupResponse(
            group.GroupId,
            group.GroupName,
            group.GroupAvatar,
            group.GroupDescription,
            group.GroupType!,
            group.CreatedBy,
            group.CreatedAt!.Value,
            group.LastMessageAt,
            group.IsActive!.Value,
            1
        );
    }
}
