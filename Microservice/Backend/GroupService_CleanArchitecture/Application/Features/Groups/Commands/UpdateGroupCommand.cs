using Application.Common;
using Application.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Groups.Commands;

public record UpdateGroupCommand(int GroupId, string? GroupName, string? GroupAvatar, string? GroupDescription, string UserId) 
    : ICommand<GroupResponse>;

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, GroupResponse>
{
    private readonly IGroupRepository _groupRepository;

    public UpdateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupResponse> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        
        if (group == null)
            throw new KeyNotFoundException($"Group with ID {request.GroupId} not found");

        // Check if user is owner or admin
        if (!await _groupRepository.IsUserOwnerOrAdminAsync(request.GroupId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("Only owners and admins can update group details");

        // Update group properties
        if (request.GroupName != null)
            group.GroupName = request.GroupName;
        
        if (request.GroupAvatar != null)
            group.GroupAvatar = request.GroupAvatar;
        
        if (request.GroupDescription != null)
            group.GroupDescription = request.GroupDescription;

        group.UpdatedAt = DateTime.UtcNow;

        await _groupRepository.UpdateAsync(group, cancellationToken);

        return new GroupResponse(
            group.GroupId,
            group.GroupName,
            group.GroupAvatar,
            group.GroupDescription,
            group.GroupType.ToString().ToLower(),
            group.CreatedBy,
            group.CreatedAt,
            group.LastMessageAt,
            group.IsActive,
            group.GroupMembers.Count(m => m.IsActive)
        );
    }
}
