using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DirectMessageRepository : IDirectMessageRepository
{
    private readonly GroupServiceDbContext _context;

    public DirectMessageRepository(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<DirectMessageGroup?> GetByUsersAsync(string user1Id, string user2Id, CancellationToken cancellationToken = default)
    {
        return await _context.DirectMessageGroups
            .Include(dm => dm.Group)
            .FirstOrDefaultAsync(dm => dm.User1Id == user1Id && dm.User2Id == user2Id, cancellationToken);
    }

    public async Task<DirectMessageGroup> AddAsync(DirectMessageGroup directMessage, CancellationToken cancellationToken = default)
    {
        _context.DirectMessageGroups.Add(directMessage);
        await _context.SaveChangesAsync(cancellationToken);
        return directMessage;
    }

    public async Task<List<DirectMessageGroup>> GetUserDirectMessagesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.DirectMessageGroups
            .Where(dm => dm.User1Id == userId || dm.User2Id == userId)
            .Include(dm => dm.Group)
            .OrderByDescending(dm => dm.Group.LastMessageAt)
            .ToListAsync(cancellationToken);
    }
}