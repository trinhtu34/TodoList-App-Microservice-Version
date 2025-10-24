using Domain.Entities;

namespace Domain.Repositories;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(int groupId, CancellationToken cancellationToken = default);
    Task<Group?> GetByIdWithMembersAsync(int groupId, CancellationToken cancellationToken = default);
    Task<List<Group>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default);
    Task<Group> AddAsync(Group group, CancellationToken cancellationToken = default);
    Task UpdateAsync(Group group, CancellationToken cancellationToken = default);
    Task DeleteAsync(Group group, CancellationToken cancellationToken = default);
    Task<bool> IsUserMemberAsync(int groupId, string userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserOwnerOrAdminAsync(int groupId, string userId, CancellationToken cancellationToken = default);
}
