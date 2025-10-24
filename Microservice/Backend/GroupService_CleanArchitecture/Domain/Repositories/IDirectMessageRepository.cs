using Domain.Entities;

namespace Domain.Repositories;

public interface IDirectMessageRepository
{
    Task<DirectMessageGroup?> GetByUsersAsync(string user1Id, string user2Id, CancellationToken cancellationToken = default);
    Task<DirectMessageGroup> AddAsync(DirectMessageGroup directMessage, CancellationToken cancellationToken = default);
    Task<List<DirectMessageGroup>> GetUserDirectMessagesAsync(string userId, CancellationToken cancellationToken = default);
}
