using GroupService.Application.Common;
using GroupService.DTOs;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Groups.Commands;

public record UpdateGroupCommand(int GroupId, string? GroupName, string? GroupAvatar, string? GroupDescription, string UserId) 
    : ICommand<GroupResponse>;

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, GroupResponse>
{
    private readonly GroupServiceDbContext _context;

    public UpdateGroupCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<GroupResponse> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.GroupsRs
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken)
            ?? throw new Exception("Group not found");

        var member = group.GroupMembers.FirstOrDefault(m => m.UserId == request.UserId)
            ?? throw new Exception("Not a member");

        if (member.Role != "owner" && member.Role != "admin")
            throw new Exception("No permission");

        if (request.GroupName != null) group.GroupName = request.GroupName;
        if (request.GroupAvatar != null) group.GroupAvatar = request.GroupAvatar;
        if (request.GroupDescription != null) group.GroupDescription = request.GroupDescription;

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
            group.GroupMembers.Count(m => m.IsActive == true)
        );
    }
}
