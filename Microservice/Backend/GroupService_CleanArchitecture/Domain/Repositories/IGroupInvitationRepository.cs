using Domain.Entities;

namespace Domain.Repositories;

public interface IGroupInvitationRepository
{
    Task<GroupInvitation?> GetByIdAsync(int invitationId, CancellationToken cancellationToken = default);
    Task<List<GroupInvitation>> GetUserInvitationsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> CheckInvitationExists(int groupId, string InvitedUser, CancellationToken cancellationToken = default);
    Task<GroupInvitation> AddAsync(GroupInvitation invitation, CancellationToken cancellationToken = default);
    Task UpdateAsync(GroupInvitation invitation, CancellationToken cancellationToken = default);
    Task DeleteAsync(GroupInvitation invitation, CancellationToken cancellationToken = default);
}
