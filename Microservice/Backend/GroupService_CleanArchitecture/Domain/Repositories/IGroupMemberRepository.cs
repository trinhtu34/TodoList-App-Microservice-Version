using Domain.Entities;

namespace Domain.Repositories;

public interface IGroupMemberRepository
{
    Task<GroupMember?> GetByIdAsync(int groupId, string userId, CancellationToken cancellationToken = default);
    Task<List<GroupMember>> GetGroupMembersAsync(int groupId, CancellationToken cancellationToken = default);
    Task<GroupMember> AddAsync(GroupMember member, CancellationToken cancellationToken = default);
    Task UpdateAsync(GroupMember member, CancellationToken cancellationToken = default);
    Task DeleteAsync(GroupMember member, CancellationToken cancellationToken = default);
}
