using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GroupInvitationRepository : IGroupInvitationRepository
{
    private readonly GroupServiceDbContext _context;

    public GroupInvitationRepository(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<GroupInvitation?> GetByIdAsync(int invitationId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupInvitations
            .Include(i => i.Group)
            .FirstOrDefaultAsync(i => i.InvitationId == invitationId, cancellationToken);
    }

    public async Task<List<GroupInvitation>> GetUserInvitationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupInvitations
            .Include(i => i.Group)
            .Where(i => i.InvitedUser == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckInvitationExists(int groupId, string invitedUser, CancellationToken cancellationToken = default)
    {
        return await _context.GroupInvitations
            .AnyAsync(i => i.GroupId == groupId && i.InvitedUser == invitedUser && i.Status == Domain.Enums.InvitationStatus.pending, cancellationToken);
    }

    public async Task<GroupInvitation> AddAsync(GroupInvitation invitation, CancellationToken cancellationToken = default)
    {
        _context.GroupInvitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);
        return invitation;
    }

    public async Task UpdateAsync(GroupInvitation invitation, CancellationToken cancellationToken = default)
    {
        _context.GroupInvitations.Update(invitation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GroupInvitation invitation, CancellationToken cancellationToken = default)
    {
        _context.GroupInvitations.Remove(invitation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
