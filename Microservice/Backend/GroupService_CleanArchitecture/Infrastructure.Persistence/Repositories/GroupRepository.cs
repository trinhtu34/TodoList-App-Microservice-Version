using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly GroupServiceDbContext _context;

    public GroupRepository(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == groupId, cancellationToken);
    }

    public async Task<Group?> GetByIdWithMembersAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.GroupMembers.Where(m => m.IsActive))
            .FirstOrDefaultAsync(g => g.GroupId == groupId, cancellationToken);
    }

    public async Task<List<Group>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .Where(m => m.UserId == userId && m.IsActive)
            .Include(m => m.Group)
                .ThenInclude(g => g.GroupMembers.Where(gm => gm.IsActive))
            .OrderByDescending(m => m.Group.LastMessageAt)
            .Select(m => m.Group)
            .ToListAsync(cancellationToken);
    }

    public async Task<Group> AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync(cancellationToken);
        return group;
    }

    public async Task UpdateAsync(Group group, CancellationToken cancellationToken = default)
    {
        _context.Groups.Update(group);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Group group, CancellationToken cancellationToken = default)
    {
        _context.Groups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsUserMemberAsync(int groupId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive, cancellationToken);
    }

    public async Task<bool> IsUserOwnerOrAdminAsync(int groupId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive && 
                          (m.Role == MemberRole.owner || m.Role == MemberRole.admin), cancellationToken);
    }
}
