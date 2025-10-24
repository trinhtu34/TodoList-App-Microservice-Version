using Application.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Groups.Commands;

public record DeleteGroupCommand(int GroupId, string UserId) : ICommand;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
{
    private readonly IGroupRepository _groupRepository;

    public DeleteGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        
        if (group == null)
            throw new KeyNotFoundException($"Group with ID {request.GroupId} not found");

        // Check if user is owner
        if (!await _groupRepository.IsUserOwnerOrAdminAsync(request.GroupId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("Only owners can delete groups");

        // Soft delete by setting IsActive to false
        group.IsActive = false;
        group.UpdatedAt = DateTime.UtcNow;

        await _groupRepository.UpdateAsync(group, cancellationToken);
    }
}
